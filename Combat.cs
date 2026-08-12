public class Combat
{
    public Player player { get; set; }
    public IMonster monster { get; set; }
    public Messages messages { get; set; }

    public bool PlayerRetreated { get; private set; }

    private readonly Prompt _prompt;
    private readonly Die _die;

    public Combat(Player player, IMonster monster, Messages messages, Prompt? prompt = null, Die? die = null)
    {
        this.player = player;
        this.monster = monster;
        this.messages = messages;
        _prompt = prompt ?? new Prompt(messages);
        _die = die ?? new Die();
        PlayerRetreated = false;
    }

    public static readonly Dictionary<string, string> AttackDrinkOrRetreat = new()
    {
        ["a"] = "attack",
        ["attack"] = "attack",
        ["d"] = "drink",
        ["drink"] = "drink",
        ["r"] = "retreat",
        ["retreat"] = "retreat"
    };

    public bool StartCombat()
    {
        Console.WriteLine(GetCombatStatsDisplay());

        bool skipPlayerAttack = false;

        while (true)
        {
            // Drinking last round cost this round's swing. The monster's turn
            // below still runs, so healing is a trade rather than a free action.
            if (!skipPlayerAttack)
            {
                PlayerAttacksMonsterSequence();

                if (monster.HealthPoints <= 0)
                    return true;
            }
            skipPlayerAttack = false;

            MonsterAttacksPlayerSequence();

            if (player.HealthPoints <= 0)
                return false;

            // Drinking is only offered when a potion is actually carried, the
            // same way the shop only lists what is in stock.
            string chosen = player.HealingPotions > 0
                ? _prompt.AskChoice("combat_prompt_with_potion", AttackDrinkOrRetreat)
                : _prompt.AskChoice("attack_prompt", Game.AttackOrRetreat);

            if (chosen == "retreat")
            {
                PlayerRetreated = true;
                Console.WriteLine(messages.GetMessage("retreat_combat"));
                return false;
            }

            if (chosen == "drink")
                skipPlayerAttack = DrinkSequence();
        }
    }

    // Returns whether the round was actually spent. A refused drink costs
    // nothing, so a player at full health does not lose a swing for asking.
    private bool DrinkSequence()
    {
        Player.DrinkOutcome outcome = player.DrinkHealingPotion(_die.Roll(10), out int restored);

        if (outcome == Player.DrinkOutcome.Healed)
        {
            Console.WriteLine(string.Format(
                messages.GetMessage("potion_healed"), restored, player.HealthPoints));
            return true;
        }

        Console.WriteLine(messages.GetMessage(
            outcome == Player.DrinkOutcome.AlreadyAtFullHealth
                ? "potion_at_full_health"
                : "potion_none_carried"));

        return false;
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