public enum PurchaseOutcome
{
    Purchased,
    InsufficientGold,
    AlreadyOwned
}

// Shop logic for the Wandering Merchant. Merchants are stateless per
// ADR-0002: ownership lives on the Player, never on the merchant, so
// every encounter is a fresh individual.
public class Merchant
{
    public const int Price = 30;
    public const int HaggleDiscount = 10;
    public const string EnchantedSwordType = "enchanted sword";
    public const string EnchantedArmorType = "enchanted armor";
    public const string FistsType = "fists";

    private readonly Die _die;

    public Merchant(Die die)
    {
        _die = die;
    }

    // The Encounter Roll: a d20 result of 16-20 (25%) means a Wandering
    // Merchant appears on the South path.
    public bool RollEncounter()
    {
        return _die.Roll(20) >= 16;
    }

    public bool OffersEnchantedSword(Player player)
    {
        return player.Weapon == null || player.Weapon.Type != EnchantedSwordType;
    }

    public bool OffersEnchantedArmor(Player player)
    {
        return player.Armor == null;
    }

    public PurchaseOutcome BuyEnchantedSword(Player player)
    {
        return BuyEnchantedSword(player, Price);
    }

    public PurchaseOutcome BuyEnchantedSword(Player player, int price)
    {
        if (!OffersEnchantedSword(player))
            return PurchaseOutcome.AlreadyOwned;
        if (player.Gold < price)
            return PurchaseOutcome.InsufficientGold;
        player.Gold -= price;
        player.Weapon = new Weapon(EnchantedSwordType, 16, "|*====>");
        return PurchaseOutcome.Purchased;
    }

    public PurchaseOutcome BuyEnchantedArmor(Player player)
    {
        return BuyEnchantedArmor(player, Price);
    }

    public PurchaseOutcome BuyEnchantedArmor(Player player, int price)
    {
        if (!OffersEnchantedArmor(player))
            return PurchaseOutcome.AlreadyOwned;
        if (player.Gold < price)
            return PurchaseOutcome.InsufficientGold;
        player.Gold -= price;
        player.Armor = new Armor(EnchantedArmorType, 3);
        return PurchaseOutcome.Purchased;
    }

    // Fists and the Enchanted Sword cannot be sold, so the economy
    // can't be exploited in a loop.
    public bool CanSellWeapon(Player player)
    {
        return player.Weapon != null
            && player.Weapon.Type != FistsType
            && player.Weapon.Type != EnchantedSwordType;
    }

    // Sells the player's current weapon for its MaxDamage in Gold,
    // leaving them fighting with Fists. Returns the Gold credited,
    // or 0 when the weapon cannot be sold.
    public int SellWeapon(Player player)
    {
        if (!CanSellWeapon(player))
            return 0;
        int credited = player.Weapon!.MaxDamage;
        player.Gold += credited;
        player.Weapon = new Weapon(FistsType, 4, "##");
        return credited;
    }

    // The Haggle gamble: one d20 roll, success at or under the player's
    // Agility. The caller applies the discount on success and must end
    // the entire encounter on failure.
    public bool Haggle(Player player, out int roll)
    {
        roll = _die.Roll(20);
        return roll <= player.Agility;
    }
}
