// What the combat loop needs from an opponent. The Dragon is one implementation;
// anything else the player can fight is another. Everything that belongs to a
// specific creature — its taunt keys, how its stats are rolled — stays on that
// creature and never appears here.
public interface IMonster
{
    string Name { get; }
    int Strength { get; }
    int Agility { get; }

    // Settable: the player's attack subtracts damage from it directly.
    int HealthPoints { get; set; }

    Weapon Weapon { get; }

    string Attack(Player player, Messages messages);
    string GetRandomDefenseTaunt(Messages messages);
    string GetRandomDamageReply(Messages messages);
    void DisplayStats(Messages messages);

    // Each creature supplies its own death line, so the player's attack never
    // has to know which one it just killed.
    string GetDefeatedNarrative(Messages messages);
}
