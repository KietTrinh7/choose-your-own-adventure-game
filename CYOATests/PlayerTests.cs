[TestClass]
public class PlayerTests
{
    [TestMethod]
    public void NewPlayer_ShouldHaveDefaultValues()
    {
        Player player = new Player();

        Assert.AreEqual("", player.Name);
        Assert.AreEqual("", player.Race);
        Assert.AreEqual("", player.Occupation);

        Assert.AreEqual(0, player.Strength);
        Assert.AreEqual(0, player.Agility);
        Assert.AreEqual(0, player.HealthPoints);

        Assert.IsNull(player.Weapon);
    }

    // ── Gold and Armor primitives (merchant-encounter ticket 01) ─────────────

    [TestMethod]
    public void NewPlayer_StartsWithExactly50Gold()
    {
        Player player = new Player();

        Assert.AreEqual(50, player.Gold);
    }

    [TestMethod]
    public void NewPlayer_HasNoArmor()
    {
        Player player = new Player();

        Assert.IsNull(player.Armor);
    }

    [TestMethod]
    public void ReduceDamage_WithProtection3_Raw8_Returns5()
    {
        Player player = new Player { Armor = new Armor("enchanted armor", 3) };

        Assert.AreEqual(5, player.ReduceDamage(8));
    }

    [TestMethod]
    public void ReduceDamage_WithProtection3_Raw2_Returns0()
    {
        Player player = new Player { Armor = new Armor("enchanted armor", 3) };

        Assert.AreEqual(0, player.ReduceDamage(2));
    }

    [TestMethod]
    public void ReduceDamage_WithNoArmor_ReturnsRawDamageUnchanged()
    {
        Player player = new Player();

        Assert.AreEqual(7, player.ReduceDamage(7));
    }
}
