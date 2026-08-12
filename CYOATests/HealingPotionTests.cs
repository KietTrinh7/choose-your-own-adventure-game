[TestClass]
public class HealingPotionTests
{
    private class FixedDie : Die
    {
        private readonly int _value;
        public FixedDie(int value) { _value = value; }
        public override int Roll(int sides) => _value;
    }

    private static string NewTempDir()
    {
        string dir = Path.Combine(Path.GetTempPath(), "cyoa-potion-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static Player WoundedHero(int health = 5, int max = 20, int potions = 1)
    {
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = health,
            MaxHealthPoints = max,
            HealingPotions = potions,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    // ── Healing maths ────────────────────────────────────────────────────────

    [TestMethod]
    public void Drinking_RestoresTheRolledAmount()
    {
        Player player = WoundedHero(health: 5, max: 20);

        Player.DrinkOutcome outcome = player.DrinkHealingPotion(7, out int restored);

        Assert.AreEqual(Player.DrinkOutcome.Healed, outcome);
        Assert.AreEqual(7, restored);
        Assert.AreEqual(12, player.HealthPoints);
    }

    [TestMethod]
    public void Drinking_NeverHealsAboveTheMaximumRolledAtCreation()
    {
        Player player = WoundedHero(health: 18, max: 20);

        player.DrinkHealingPotion(30, out int restored);

        Assert.AreEqual(20, player.HealthPoints);
        Assert.AreEqual(2, restored);
    }

    [TestMethod]
    public void Drinking_AtFullHealth_IsRefusedAndKeepsThePotion()
    {
        Player player = WoundedHero(health: 20, max: 20, potions: 3);

        Player.DrinkOutcome outcome = player.DrinkHealingPotion(9, out int restored);

        Assert.AreEqual(Player.DrinkOutcome.AlreadyAtFullHealth, outcome);
        Assert.AreEqual(0, restored);
        Assert.AreEqual(3, player.HealingPotions);
        Assert.AreEqual(20, player.HealthPoints);
    }

    [TestMethod]
    public void Drinking_WithNonePreparedCarried_ChangesNothing()
    {
        Player player = WoundedHero(health: 5, max: 20, potions: 0);

        Player.DrinkOutcome outcome = player.DrinkHealingPotion(9, out int restored);

        Assert.AreEqual(Player.DrinkOutcome.NonePreparedCarried, outcome);
        Assert.AreEqual(0, restored);
        Assert.AreEqual(5, player.HealthPoints);
    }

    [TestMethod]
    public void EachSuccessfulDrink_ConsumesExactlyOnePotion()
    {
        Player player = WoundedHero(health: 1, max: 20, potions: 3);

        player.DrinkHealingPotion(2, out _);
        Assert.AreEqual(2, player.HealingPotions);

        player.DrinkHealingPotion(2, out _);
        Assert.AreEqual(1, player.HealingPotions);
    }

    // ── Buying ───────────────────────────────────────────────────────────────

    [TestMethod]
    public void BuyingAPotion_DeductsThePriceAndIncrementsTheCount()
    {
        Player player = WoundedHero(potions: 0);
        player.Gold = 50;

        PurchaseOutcome outcome = new Merchant(new Die()).BuyHealingPotion(player);

        Assert.AreEqual(PurchaseOutcome.Purchased, outcome);
        Assert.AreEqual(40, player.Gold);
        Assert.AreEqual(1, player.HealingPotions);
    }

    [TestMethod]
    public void BuyingAPotion_WithoutEnoughGold_ChangesNothing()
    {
        Player player = WoundedHero(potions: 0);
        player.Gold = 3;

        PurchaseOutcome outcome = new Merchant(new Die()).BuyHealingPotion(player);

        Assert.AreEqual(PurchaseOutcome.InsufficientGold, outcome);
        Assert.AreEqual(3, player.Gold);
        Assert.AreEqual(0, player.HealingPotions);
    }

    // This is what distinguishes a potion from the one-of-a-kind goods.
    [TestMethod]
    public void PotionsAreOrdinaryStock_BoughtRepeatedlyAndAlwaysOffered()
    {
        Player player = WoundedHero(potions: 0);
        player.Gold = 50;
        var merchant = new Merchant(new Die());

        merchant.BuyHealingPotion(player);
        merchant.BuyHealingPotion(player);
        merchant.BuyHealingPotion(player);

        Assert.AreEqual(3, player.HealingPotions);
        Assert.AreEqual(20, player.Gold);
        Assert.IsTrue(merchant.OffersHealingPotion(player));
    }

    [TestMethod]
    public void FiftyGold_BuysOneMagicalItemAndTwoPotions()
    {
        Player player = WoundedHero(potions: 0);
        player.Gold = 50;
        player.Armor = null;
        var merchant = new Merchant(new Die());

        merchant.BuyEnchantedArmor(player, Merchant.Price);
        merchant.BuyHealingPotion(player);
        merchant.BuyHealingPotion(player);

        Assert.AreEqual(0, player.Gold);
        Assert.AreEqual(2, player.HealingPotions);
        Assert.IsNotNull(player.Armor);
    }

    // ── Persistence ──────────────────────────────────────────────────────────

    [TestMethod]
    public void PotionsAndMaximumHealth_SurviveAProfileRoundTrip()
    {
        var store = new ProfileStore(NewTempDir());
        store.Save(WoundedHero(health: 7, max: 23, potions: 4));

        Player loaded = store.Load("TestHero")!;

        Assert.AreEqual(23, loaded.MaxHealthPoints);
        Assert.AreEqual(4, loaded.HealingPotions);

        loaded.DrinkHealingPotion(100, out _);
        Assert.AreEqual(23, loaded.HealthPoints);
    }

    // Profiles written before maximum health existed have no value for it.
    [TestMethod]
    public void AProfileSavedBeforeMaximumHealthExisted_LoadsWithAUsableCeiling()
    {
        string dir = NewTempDir();
        File.WriteAllText(Path.Combine(dir, "profiles.json"),
            "{\"OldHero\":{\"Name\":\"OldHero\",\"Race\":\"Human\",\"Occupation\":\"Fighter\"," +
            "\"Strength\":15,\"Agility\":12,\"HealthPoints\":18,\"Gold\":50," +
            "\"Weapon\":null,\"Armor\":null}}");

        Player loaded = new ProfileStore(dir).Load("OldHero")!;

        Assert.AreEqual(18, loaded.MaxHealthPoints);
    }

    // ── Localization ─────────────────────────────────────────────────────────

    [TestMethod]
    public void PotionMessages_ResolveInEveryLanguage()
    {
        string[] keys =
        {
            "shop_option_potion", "stats_potions", "combat_prompt_with_potion",
            "potion_healed", "potion_at_full_health", "potion_none_carried"
        };

        foreach (string language in new[] { "English", "Spanish", "French", "Italian" })
        {
            Messages messages = new Messages();
            messages.SetCurrentLanguage(language);
            messages.ReadDictionary();

            foreach (string key in keys)
                Assert.IsFalse(messages.GetMessage(key).StartsWith("["), $"{key} is missing from {language}");
        }
    }
}
