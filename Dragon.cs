// The boss at the end of the North path. Rolls every stat on a d20 and hits
// with claws for up to 12. Fighting it ends the run in every outcome.
public class Dragon : Monster
{
    private static readonly string[] _defenseTauntKeys =
    {
        "dragon_defense_taunt_1",
        "dragon_defense_taunt_2",
        "dragon_defense_taunt_3",
        "dragon_defense_taunt_4"
    };

    private static readonly string[] _damageReplyKeys =
    {
        "dragon_damage_reply_1",
        "dragon_damage_reply_2",
        "dragon_damage_reply_3",
        "dragon_damage_reply_4"
    };

    public Dragon(Die die)
        : base(die, "Smolderfang", new Weapon("claws", 12, "<<< claws >>>"))
    {
        Strength = die.Roll(20);
        Agility = die.Roll(20);
        HealthPoints = die.Roll(20);
    }

    protected override IReadOnlyList<string> DefenseTauntKeys => _defenseTauntKeys;
    protected override IReadOnlyList<string> DamageReplyKeys => _damageReplyKeys;
    protected override string StatsHeaderKey => "dragon_stats_header";
    protected override string DefeatedNarrativeKey => "dragon_defeated_narrative";
}
