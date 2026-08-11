[TestClass]
public class ProfileStoreTests
{
    // Each test gets its own directory so the real user data folder is never touched.
    private static string NewTempDir()
    {
        string dir = Path.Combine(Path.GetTempPath(), "cyoa-tests-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static Player CreateFighter(string name = "TestHero")
    {
        return new Player
        {
            Name = name,
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Gold = 37,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    [TestMethod]
    public void SaveThenLoad_RestoresEveryField()
    {
        var store = new ProfileStore(NewTempDir());
        store.Save(CreateFighter());

        Player? loaded = store.Load("TestHero");

        Assert.IsNotNull(loaded);
        Assert.AreEqual("TestHero", loaded!.Name);
        Assert.AreEqual("Human", loaded.Race);
        Assert.AreEqual("Fighter", loaded.Occupation);
        Assert.AreEqual(15, loaded.Strength);
        Assert.AreEqual(12, loaded.Agility);
        Assert.AreEqual(25, loaded.HealthPoints);
        Assert.AreEqual(37, loaded.Gold);
        Assert.AreEqual("long sword", loaded.Weapon!.Type);
        Assert.AreEqual(12, loaded.Weapon.MaxDamage);
        Assert.AreEqual("-)=====>", loaded.Weapon.AsciiArt);
    }

    [TestMethod]
    public void SaveThenLoad_WithNoWeaponAndNoArmor_RestoresBothAsNull()
    {
        var store = new ProfileStore(NewTempDir());
        Player barehanded = CreateFighter();
        barehanded.Weapon = null;
        barehanded.Armor = null;

        store.Save(barehanded);
        Player? loaded = store.Load("TestHero");

        Assert.IsNotNull(loaded);
        Assert.IsNull(loaded!.Weapon);
        Assert.IsNull(loaded.Armor);
    }

    [TestMethod]
    public void SaveThenLoad_WithEnchantedArmor_KeepsProtectionWorkingInCombat()
    {
        var store = new ProfileStore(NewTempDir());
        Player armored = CreateFighter();
        armored.Armor = new Armor("enchanted armor", 3);

        store.Save(armored);
        Player? loaded = store.Load("TestHero");

        Assert.IsNotNull(loaded);
        Assert.AreEqual("enchanted armor", loaded!.Armor!.Type);
        Assert.AreEqual(3, loaded.Armor.Protection);
        // The armor still reduces damage after a reload, so combat is unchanged.
        Assert.AreEqual(5, loaded.ReduceDamage(8));
        Assert.AreEqual(0, loaded.ReduceDamage(2));
    }

    [TestMethod]
    public void ListNames_WithNoProfiles_IsEmpty()
    {
        var store = new ProfileStore(NewTempDir());

        Assert.AreEqual(0, store.ListNames().Count);
    }

    [TestMethod]
    public void ListNames_ReturnsEverySavedProfile()
    {
        var store = new ProfileStore(NewTempDir());
        store.Save(CreateFighter("Aragorn"));
        store.Save(CreateFighter("Bilbo"));

        var names = store.ListNames();

        Assert.AreEqual(2, names.Count);
        CollectionAssert.Contains(names, "Aragorn");
        CollectionAssert.Contains(names, "Bilbo");
    }

    [TestMethod]
    public void Save_UnderAnExistingName_ReplacesOnlyThatProfile()
    {
        var store = new ProfileStore(NewTempDir());
        store.Save(CreateFighter("Aragorn"));
        store.Save(CreateFighter("Bilbo"));

        Player replacement = CreateFighter("Aragorn");
        replacement.Gold = 999;
        store.Save(replacement);

        Assert.AreEqual(999, store.Load("Aragorn")!.Gold);
        Assert.AreEqual(37, store.Load("Bilbo")!.Gold);
        Assert.AreEqual(2, store.ListNames().Count);
    }

    // The one-of-a-kind rules are derived from the player's weapon and armor
    // (ADR-0002), so persisting those two slots must persist the rules with them.
    [TestMethod]
    public void ReloadedOwnerOfEnchantedGear_IsNotOfferedItAgain()
    {
        var store = new ProfileStore(NewTempDir());
        Player kitted = CreateFighter();
        kitted.Weapon = new Weapon("enchanted sword", 16, "-)=====>");
        kitted.Armor = new Armor("enchanted armor", 3);
        store.Save(kitted);

        Player loaded = store.Load("TestHero")!;
        var merchant = new Merchant(new Die());

        Assert.IsFalse(merchant.OffersEnchantedSword(loaded));
        Assert.IsFalse(merchant.OffersEnchantedArmor(loaded));
    }

    [TestMethod]
    public void ReloadedPlayerHoldingFists_HasNoSellOption()
    {
        var store = new ProfileStore(NewTempDir());
        Player unarmed = CreateFighter();
        unarmed.Weapon = new Weapon("fists", 4, "");
        store.Save(unarmed);

        Player loaded = store.Load("TestHero")!;

        Assert.IsFalse(new Merchant(new Die()).CanSellWeapon(loaded));
    }

    [TestMethod]
    public void Exists_IsTrueOnlyForSavedNames()
    {
        var store = new ProfileStore(NewTempDir());
        store.Save(CreateFighter("Aragorn"));

        Assert.IsTrue(store.Exists("Aragorn"));
        Assert.IsFalse(store.Exists("Bilbo"));
    }
}
