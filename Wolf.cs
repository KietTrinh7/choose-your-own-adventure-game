// The wilderness threat on the East path. Rolls every stat on a d10 against the
// dragon's d20 and bites for at most 6 against the dragon's 12, so it can never
// be the tougher fight. Beating it or fleeing it returns the player to the
// adventure menu; only dying to it ends the run.
public class Wolf : Monster
{
    public const int StatDie = 10;
    public const int FangsMaxDamage = 6;

    private static readonly string[] _defenseTauntKeys =
    {
        "wolf_defense_taunt_1",
        "wolf_defense_taunt_2"
    };

    private static readonly string[] _damageReplyKeys =
    {
        "wolf_damage_reply_1",
        "wolf_damage_reply_2"
    };

    public Wolf(Die die)
        : base(die, "Greyfang", new Weapon("fangs", FangsMaxDamage, ">>> fangs <<<"))
    {
        Strength = die.Roll(StatDie);
        Agility = die.Roll(StatDie);
        HealthPoints = die.Roll(StatDie);
    }

    protected override IReadOnlyList<string> DefenseTauntKeys => _defenseTauntKeys;
    protected override IReadOnlyList<string> DamageReplyKeys => _damageReplyKeys;
    protected override string StatsHeaderKey => "wolf_stats_header";
    protected override string DefeatedNarrativeKey => "wolf_defeated_narrative";

    // Same Encounter Roll rule the Wandering Merchant uses on South.
    public bool RollEncounter()
    {
        return _die.Roll(20) >= 16;
    }
}
