[TestClass]
public class MerchantTests
{
    // A Die whose rolls always land on a fixed value.
    private class FixedDie : Die
    {
        private readonly int _value;
        public FixedDie(int value) { _value = value; }
        public override int Roll(int sides) => _value;
    }

    private Player CreateFighter()
    {
        // Fighter starts with a long sword (MaxDamage 12) and 50 Gold.
        return new Player
        {
            Name = "TestHero",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 15,
            Agility = 12,
            HealthPoints = 25,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }

    private Merchant CreateMerchant(int fixedRoll = 10)
    {
        return new Merchant(new FixedDie(fixedRoll));
    }

    // ── Encounter Roll ───────────────────────────────────────────────────────

    [TestMethod]
    public void RollEncounter_Of16_ProducesMerchant()
    {
        Assert.IsTrue(new Merchant(new FixedDie(16)).RollEncounter());
    }

    [TestMethod]
    public void RollEncounter_Of20_ProducesMerchant()
    {
        Assert.IsTrue(new Merchant(new FixedDie(20)).RollEncounter());
    }

    [TestMethod]
    public void RollEncounter_Of15_ProducesNoMerchant()
    {
        Assert.IsFalse(new Merchant(new FixedDie(15)).RollEncounter());
    }

    [TestMethod]
    public void RollEncounter_Of1_ProducesNoMerchant()
    {
        Assert.IsFalse(new Merchant(new FixedDie(1)).RollEncounter());
    }

    // ── Buying the Enchanted Sword ───────────────────────────────────────────

    [TestMethod]
    public void BuyEnchantedSword_WithEnoughGold_DeductsExactly30AndEquips()
    {
        Player player = CreateFighter();

        PurchaseOutcome outcome = CreateMerchant().BuyEnchantedSword(player);

        Assert.AreEqual(PurchaseOutcome.Purchased, outcome);
        Assert.AreEqual(20, player.Gold);
        Assert.AreEqual(Merchant.EnchantedSwordType, player.Weapon!.Type);
        Assert.AreEqual(16, player.Weapon.MaxDamage);
    }

    [TestMethod]
    public void BuyEnchantedSword_WithInsufficientGold_RefusesWithNoStateChange()
    {
        Player player = CreateFighter();
        player.Gold = 29;

        PurchaseOutcome outcome = CreateMerchant().BuyEnchantedSword(player);

        Assert.AreEqual(PurchaseOutcome.InsufficientGold, outcome);
        Assert.AreEqual(29, player.Gold);
        Assert.AreEqual("long sword", player.Weapon!.Type);
    }

    // ── Buying the Enchanted Armor ───────────────────────────────────────────

    [TestMethod]
    public void BuyEnchantedArmor_WithEnoughGold_DeductsExactly30AndEquips()
    {
        Player player = CreateFighter();

        PurchaseOutcome outcome = CreateMerchant().BuyEnchantedArmor(player);

        Assert.AreEqual(PurchaseOutcome.Purchased, outcome);
        Assert.AreEqual(20, player.Gold);
        Assert.IsNotNull(player.Armor);
        Assert.AreEqual(3, player.Armor!.Protection);
    }

    [TestMethod]
    public void BuyEnchantedArmor_WithInsufficientGold_RefusesWithNoStateChange()
    {
        Player player = CreateFighter();
        player.Gold = 0;

        PurchaseOutcome outcome = CreateMerchant().BuyEnchantedArmor(player);

        Assert.AreEqual(PurchaseOutcome.InsufficientGold, outcome);
        Assert.AreEqual(0, player.Gold);
        Assert.IsNull(player.Armor);
    }

    // ── Owned items are never offered again ──────────────────────────────────

    [TestMethod]
    public void OffersEnchantedSword_AfterPurchase_IsFalseEvenForFreshMerchant()
    {
        Player player = CreateFighter();
        Merchant merchant = CreateMerchant();
        Assert.IsTrue(merchant.OffersEnchantedSword(player));

        merchant.BuyEnchantedSword(player);

        // Ownership lives on the Player (ADR-0002), so a fresh merchant also refuses.
        Assert.IsFalse(merchant.OffersEnchantedSword(player));
        Assert.IsFalse(CreateMerchant().OffersEnchantedSword(player));
    }

    [TestMethod]
    public void OffersEnchantedArmor_AfterPurchase_IsFalseEvenForFreshMerchant()
    {
        Player player = CreateFighter();
        Merchant merchant = CreateMerchant();
        Assert.IsTrue(merchant.OffersEnchantedArmor(player));

        merchant.BuyEnchantedArmor(player);

        Assert.IsFalse(merchant.OffersEnchantedArmor(player));
        Assert.IsFalse(CreateMerchant().OffersEnchantedArmor(player));
    }

    [TestMethod]
    public void BuyEnchantedSword_WhenAlreadyOwned_ReturnsAlreadyOwnedWithNoCharge()
    {
        Player player = CreateFighter();
        player.Gold = 100;
        Merchant merchant = CreateMerchant();
        merchant.BuyEnchantedSword(player);
        int goldAfterFirstBuy = player.Gold;

        PurchaseOutcome outcome = merchant.BuyEnchantedSword(player);

        Assert.AreEqual(PurchaseOutcome.AlreadyOwned, outcome);
        Assert.AreEqual(goldAfterFirstBuy, player.Gold);
    }

    // ── Sell ─────────────────────────────────────────────────────────────────

    [TestMethod]
    public void SellWeapon_Fighter_Credits12GoldAndLeavesFists()
    {
        Player player = CreateFighter();

        int credited = CreateMerchant().SellWeapon(player);

        Assert.AreEqual(12, credited);
        Assert.AreEqual(62, player.Gold);
        Assert.AreEqual(Merchant.FistsType, player.Weapon!.Type);
        Assert.AreEqual(4, player.Weapon.MaxDamage);
    }

    [TestMethod]
    public void CanSellWeapon_WithFists_IsFalseAndSellingChangesNothing()
    {
        Player player = CreateFighter();
        Merchant merchant = CreateMerchant();
        merchant.SellWeapon(player); // now holding fists
        int goldBefore = player.Gold;

        Assert.IsFalse(merchant.CanSellWeapon(player));
        Assert.AreEqual(0, merchant.SellWeapon(player));
        Assert.AreEqual(goldBefore, player.Gold);
        Assert.AreEqual(Merchant.FistsType, player.Weapon!.Type);
    }

    [TestMethod]
    public void CanSellWeapon_WithEnchantedSword_IsFalseAndSellingChangesNothing()
    {
        Player player = CreateFighter();
        Merchant merchant = CreateMerchant();
        merchant.BuyEnchantedSword(player);
        int goldBefore = player.Gold;

        Assert.IsFalse(merchant.CanSellWeapon(player));
        Assert.AreEqual(0, merchant.SellWeapon(player));
        Assert.AreEqual(goldBefore, player.Gold);
        Assert.AreEqual(Merchant.EnchantedSwordType, player.Weapon!.Type);
    }

    // ── Haggle ───────────────────────────────────────────────────────────────

    [TestMethod]
    public void Haggle_RollEqualToAgility_Succeeds()
    {
        Player player = CreateFighter();
        player.Agility = 10;

        bool result = new Merchant(new FixedDie(10)).Haggle(player, out int roll);

        Assert.IsTrue(result);
        Assert.AreEqual(10, roll);
    }

    [TestMethod]
    public void Haggle_RollAboveAgility_Fails()
    {
        Player player = CreateFighter();
        player.Agility = 10;

        bool result = new Merchant(new FixedDie(11)).Haggle(player, out int roll);

        Assert.IsFalse(result);
        Assert.AreEqual(11, roll);
    }

    [TestMethod]
    public void BuyEnchantedSword_AtHaggledPrice_Deducts20()
    {
        Player player = CreateFighter();

        PurchaseOutcome outcome = CreateMerchant()
            .BuyEnchantedSword(player, Merchant.Price - Merchant.HaggleDiscount);

        Assert.AreEqual(PurchaseOutcome.Purchased, outcome);
        Assert.AreEqual(30, player.Gold);
    }

    // ── Economy invariant: sell-then-buy-both ────────────────────────────────

    [TestMethod]
    public void SellA12DamageWeapon_ThenBuyBothItems_Succeeds()
    {
        // The earned exception from the spec: 50 + 12 = 62 >= 60
        Player player = CreateFighter();
        Merchant merchant = CreateMerchant();

        merchant.SellWeapon(player);
        PurchaseOutcome swordOutcome = merchant.BuyEnchantedSword(player);
        PurchaseOutcome armorOutcome = merchant.BuyEnchantedArmor(player);

        Assert.AreEqual(PurchaseOutcome.Purchased, swordOutcome);
        Assert.AreEqual(PurchaseOutcome.Purchased, armorOutcome);
        Assert.AreEqual(2, player.Gold);
        Assert.AreEqual(Merchant.EnchantedSwordType, player.Weapon!.Type);
        Assert.IsNotNull(player.Armor);
    }

    // ── Localization: every new key resolves in all four languages ───────────

    [TestMethod]
    public void MerchantMessages_ResolveInAllFourLanguages()
    {
        string[] merchantKeys =
        {
            "stats_gold", "stats_armor", "armor_none", "armor_display", "armor_absorbs",
            "merchant_appears", "merchant_greeting", "shop_gold",
            "shop_option_sword", "shop_option_armor", "shop_option_sell", "shop_option_leave",
            "shop_invalid", "weapon_discard_warning", "buy_cancelled",
            "buy_success", "buy_insufficient", "merchant_farewell",
            "sell_success", "haggle_prompt", "haggle_roll", "haggle_success", "haggle_fail",
        };

        foreach (string language in new[] { "English", "Spanish", "French", "Italian" })
        {
            Messages messages = new Messages();
            messages.SetCurrentLanguage(language);
            messages.ReadDictionary();

            foreach (string key in merchantKeys)
            {
                string result = messages.GetMessage(key);
                Assert.IsFalse(result.StartsWith("["),
                    $"Key '{key}' is missing for {language}");
            }
        }
    }

    [TestMethod]
    public void ItemNames_TranslateInAllFourLanguages()
    {
        foreach (string language in new[] { "English", "Spanish", "French", "Italian" })
        {
            Messages messages = new Messages();
            messages.SetCurrentLanguage(language);
            messages.ReadDictionary();

            Assert.IsFalse(string.IsNullOrWhiteSpace(
                messages.TranslateWeaponForDisplay(Merchant.EnchantedSwordType)));
            Assert.IsFalse(string.IsNullOrWhiteSpace(
                messages.TranslateWeaponForDisplay(Merchant.FistsType)));
            Assert.IsFalse(string.IsNullOrWhiteSpace(
                messages.TranslateArmorForDisplay(Merchant.EnchantedArmorType)));
        }
    }
}
