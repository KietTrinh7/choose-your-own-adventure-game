public class Combat
{
    public Player player { get; set; }
    public IMonster monster { get; set; }
    public Messages messages { get; set; }

    public bool PlayerRetreated { get; private set; }

    private readonly Prompt _prompt;

    public Combat(Player player, IMonster monster, Messages messages, Prompt? prompt = null)
    {
        this.player = player;
        this.monster = monster;
        this.messages = messages;
        _prompt = prompt ?? new Prompt(messages);
        PlayerRetreated = false;
    }

    public bool StartCombat()
    {
        Console.WriteLine(GetCombatStatsDisplay());

        while (true)
        {
            PlayerAttacksMonsterSequence();

            if (monster.HealthPoints <= 0)
                return true;

            MonsterAttacksPlayerSequence();

            if (player.HealthPoints <= 0)
                return false;

            if (_prompt.AskChoice("attack_prompt", Game.AttackOrRetreat) == "retreat")
            {
                PlayerRetreated = true;
                Console.WriteLine(messages.GetMessage("retreat_combat"));
                return false;
            }
        }
    }

    public string GetCombatStatsDisplay()
    {
        var output = new List<string>();

        output.Add(messages.GetMessage("combat_stats_header"));
        output.Add(messages.GetMessage("combat_player_header"));
        output.Add(messages.GetMessage("stats_name") + player.Name);
        output.Add(messages.GetMessage("stats_strength") + player.Strength);
        output.Add(messages.GetMessage("stats_agility") + player.Agility);
        output.Add(messages.GetMessage("stats_health") + player.HealthPoints);

        if (player.Weapon != null)
        {
            output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(player.Weapon.Type));
            output.Add(messages.GetMessage("stats_damage") + player.Weapon.MaxDamage);
        }

        output.Add("");
        // Labelled by the creature's own name, so the same display serves any monster.
        output.Add(string.Format(messages.GetMessage("combat_monster_header"), monster.Name));
        output.Add(messages.GetMessage("stats_name") + monster.Name);
        output.Add(messages.GetMessage("stats_strength") + monster.Strength);
        output.Add(messages.GetMessage("stats_agility") + monster.Agility);
        output.Add(messages.GetMessage("stats_health") + monster.HealthPoints);
        output.Add(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(monster.Weapon.Type));
        output.Add(messages.GetMessage("stats_damage") + monster.Weapon.MaxDamage);

        return string.Join(Environment.NewLine, output);
    }

    public void PlayerAttacksMonsterSequence()
    {
        Console.WriteLine(player.Attack(monster, messages));
    }

    public void MonsterAttacksPlayerSequence()
    {
        Console.WriteLine(monster.Attack(player, messages));
    }
}