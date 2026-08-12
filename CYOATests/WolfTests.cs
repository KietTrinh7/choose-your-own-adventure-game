[TestClass]
public class WolfTests
{
    // A Die whose rolls always land on a fixed value.
    private class FixedDie : Die
    {
        private readonly int _value;
        public FixedDie(int value) { _value = value; }
        public override int Roll(int sides) => _value;
    }

    // A Die that reports the highest possible result for whatever it is given.
    private class MaxDie : Die
    {
        public override int Roll(int sides) => sides;
    }

    private Messages CreateEnglishMessages()
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage("English");
        messages.ReadDictionary();
        return messages;
    }

    // ── Stats ────────────────────────────────────────────────────────────────

    [TestMethod]
    public void Wolf_RollsEveryStatOnTheStatDie()
    {
        Wolf wolf = new Wolf(new FixedDie(7));

        Assert.AreEqual(7, wolf.Strength);
        Assert.AreEqual(7, wolf.Agility);
        Assert.AreEqual(7, wolf.HealthPoints);
    }

    [TestMethod]
    public void Wolf_FightsWithFangs()
    {
        Wolf wolf = new Wolf(new Die());

        Assert.AreEqual("fangs", wolf.Weapon.Type);
        Assert.AreEqual(6, wolf.Weapon.MaxDamage);
    }

    // The whole point of the wolf: it is the lesser fight. A wolf rolled as high
    // as it can go must still sit under a dragon rolled as high as it can go.
    [TestMethod]
    public void MaximumWolf_IsWeakerThanMaximumDragon()
    {
        Wolf wolf = new Wolf(new MaxDie());
        Dragon dragon = new Dragon(new MaxDie());

        Assert.IsTrue(wolf.Strength < dragon.Strength);
        Assert.IsTrue(wolf.Agility < dragon.Agility);
        Assert.IsTrue(wolf.HealthPoints < dragon.HealthPoints);
        Assert.IsTrue(wolf.Weapon.MaxDamage < dragon.Weapon.MaxDamage);
    }

    // ── Encounter Roll ───────────────────────────────────────────────────────

    [TestMethod]
    public void RollEncounter_Of16_ProducesWolf()
    {
        Assert.IsTrue(new Wolf(new FixedDie(16)).RollEncounter());
    }

    [TestMethod]
    public void RollEncounter_Of20_ProducesWolf()
    {
        Assert.IsTrue(new Wolf(new FixedDie(20)).RollEncounter());
    }

    [TestMethod]
    public void RollEncounter_Of15_ProducesNothing()
    {
        Assert.IsFalse(new Wolf(new FixedDie(15)).RollEncounter());
    }

    [TestMethod]
    public void RollEncounter_Of1_ProducesNothing()
    {
        Assert.IsFalse(new Wolf(new FixedDie(1)).RollEncounter());
    }

    // ── Combat behaviour ─────────────────────────────────────────────────────

    // Armor is a player concern, so it must work identically against any monster.
    [TestMethod]
    public void EnchantedArmor_ReducesTheWolfsBite()
    {
        Player player = new Player
        {
            Name = "TestHero",
            Race = "Human",
            Agility = 0, // never defends, so the bite always lands
            HealthPoints = 30,
            Armor = new Armor("enchanted armor", 3)
        };

        // Every roll lands on 6: the wolf hits, the player fails to defend, and
        // the bite does its maximum before Protection is applied.
        new Wolf(new FixedDie(6)).Attack(player, CreateEnglishMessages());

        Assert.AreEqual(27, player.HealthPoints);
    }

    [TestMethod]
    public void Wolf_SuppliesItsOwnDefeatLineRatherThanTheDragons()
    {
        Messages messages = CreateEnglishMessages();

        string wolfLine = new Wolf(new Die()).GetDefeatedNarrative(messages);
        string dragonLine = new Dragon(new Die()).GetDefeatedNarrative(messages);

        Assert.AreNotEqual(dragonLine, wolfLine);
        Assert.AreEqual(messages.GetMessage("wolf_defeated_narrative"), wolfLine);
    }

    // Every new key must resolve in all four languages. A missing key comes back
    // wrapped in brackets, so a bracket check catches gaps.
    [TestMethod]
    public void WolfMessages_ResolveInEveryLanguage()
    {
        string[] keys =
        {
            "wolf_appears", "wolf_stats_intro", "wolf_stats_header",
            "wolf_defense_taunt_1", "wolf_defense_taunt_2",
            "wolf_damage_reply_1", "wolf_damage_reply_2",
            "wolf_defeated_narrative", "wolf_retreat", "east_path_narrative"
        };

        foreach (string language in new[] { "English", "Spanish", "French", "Italian" })
        {
            Messages messages = new Messages();
            messages.SetCurrentLanguage(language);
            messages.ReadDictionary();

            foreach (string key in keys)
            {
                string value = messages.GetMessage(key);
                Assert.IsFalse(value.StartsWith("["), $"{key} is missing from {language}");
            }
        }
    }
}
