public class Player
{
    private readonly Die _die = new Die();

    public string Name { get; set; } = "";
    public string Race { get; set; } = "";
    public string Occupation { get; set; } = "";

    public int Strength { get; set; }
    public int Agility { get; set; }
    public int HealthPoints { get; set; }

    // The Health Points rolled at creation. Set once, never changed, and the
    // ceiling healing may not pass. Nothing in the game raises it.
    public int MaxHealthPoints { get; set; }

    public int Gold { get; set; } = 50;

    // A count, not an inventory. Part 3 chose two equipment slots and no
    // inventory system, and a consumable tally does not reopen that.
    public int HealingPotions { get; set; }
    public Weapon? Weapon { get; set; }
    public Armor? Armor { get; set; }

    // Applies armor Protection to incoming damage: max(0, raw - Protection).
    // With no armor equipped, damage passes through unchanged.
    public int ReduceDamage(int rawDamage)
    {
        if (Armor == null)
            return rawDamage;
        return Math.Max(0, rawDamage - Armor.Protection);
    }

    // nameIsTaken lets the caller veto a name that already has a Profile without
    // this class knowing anything about how Profiles are stored.
    // What happened when the player tried to drink.
    public enum DrinkOutcome { Healed, AlreadyAtFullHealth, NonePreparedCarried }

    // Restores the rolled amount, never above the ceiling set at creation. Only
    // a drink that actually heals consumes a potion. Pure apart from the roll,
    // which is why it is testable the same way ReduceDamage is.
    public DrinkOutcome DrinkHealingPotion(int roll, out int restored)
    {
        restored = 0;

        if (HealingPotions <= 0)
            return DrinkOutcome.NonePreparedCarried;

        if (HealthPoints >= MaxHealthPoints)
            return DrinkOutcome.AlreadyAtFullHealth;

        int before = HealthPoints;
        HealthPoints = Math.Min(MaxHealthPoints, HealthPoints + roll);
        restored = HealthPoints - before;
        HealingPotions--;

        return DrinkOutcome.Healed;
    }

    public void CreateCharacter(Messages messages, Prompt? prompt = null, Func<string, bool>? nameIsTaken = null)
    {
        prompt ??= new Prompt(messages);

        Race = PromptForRace(messages, prompt);
        Name = PromptForName(messages, prompt, nameIsTaken);
        Occupation = PromptForOccupation(messages, prompt);

        AssignWeaponByOccupation();

        int strengthModifier = RollStrength(messages, prompt);
        RollAgility(messages, prompt);
        RollHealthPoints(messages, prompt, strengthModifier);

        PromptForNextAction(messages, prompt);
    }

    private string PromptForRace(Messages messages, Prompt prompt)
    {
        string answer = prompt.AskText("race_prompt", messages.IsValidRace, "race_invalid");
        return messages.NormalizeRace(answer);
    }

    private string PromptForName(Messages messages, Prompt prompt, Func<string, bool>? nameIsTaken = null)
    {
        while (true)
        {
            string name = prompt.AskText("name_prompt", a => !string.IsNullOrWhiteSpace(a), "name_invalid");

            // Replacing an existing character is a decision, not a gotcha —
            // same reasoning as the warning before a weapon purchase.
            if (nameIsTaken == null || !nameIsTaken(name))
                return name;

            string warning = string.Format(messages.GetMessage("profile_overwrite_warning"), name);
            if (prompt.AskYesNo(new[] { warning }))
                return name;

            Console.WriteLine(messages.GetMessage("profile_kept"));
        }
    }

    private string PromptForOccupation(Messages messages, Prompt prompt)
    {
        string answer = prompt.AskText("occupation_prompt", messages.IsValidOccupation, "occupation_invalid");
        return messages.NormalizeOccupation(answer);
    }

    private void AssignWeaponByOccupation()
    {
        switch (Occupation.ToLower())
        {
            case "fighter":
                Weapon = new Weapon("long sword", 12, "-)=====>");
                break;
            case "magician":
                Weapon = new Weapon("lightning bolt spell", 12, "zap~~~~~~");
                break;
            case "thief":
                Weapon = new Weapon("dagger", 6, "-)==>");
                break;
            case "archer":
                Weapon = new Weapon("long bow", 8, "}    -->");
                break;
        }
    }

    private int RollStrength(Messages messages, Prompt prompt)
    {
        PromptRoll("roll_strength", prompt);
        Strength = _die.Roll(20);

        int modifier = _die.Roll(4);

        if (Race == "Halfling")
        {
            Strength -= modifier;
            return -modifier;
        }

        Strength += modifier;
        return modifier;
    }

    private void RollAgility(Messages messages, Prompt prompt)
    {
        PromptRoll("roll_agility", prompt);
        Agility = _die.Roll(20);

        if (Race == "Halfling" || Race == "Elf")
            Agility += _die.Roll(4);
    }

    private void RollHealthPoints(Messages messages, Prompt prompt, int strengthModifier)
    {
        PromptRoll("roll_health", prompt);
        HealthPoints = _die.Roll(20) + strengthModifier;
        MaxHealthPoints = HealthPoints;
    }

    private void PromptForNextAction(Messages messages, Prompt prompt)
    {
        while (true)
        {
            if (prompt.AskNumber("next_action", 2) == 2)
                return;

            DisplayStats(messages);
        }
    }

    public void DisplayStats(Messages messages)
    {
        Console.WriteLine(messages.GetMessage("stats_header"));
        Console.WriteLine(messages.GetMessage("stats_name") + Name);
        Console.WriteLine(messages.GetMessage("stats_race") + messages.TranslateRaceForDisplay(Race));
        Console.WriteLine(messages.GetMessage("stats_occupation") + messages.TranslateOccupationForDisplay(Occupation));
        Console.WriteLine(messages.GetMessage("stats_strength") + Strength);
        Console.WriteLine(messages.GetMessage("stats_agility") + Agility);
        Console.WriteLine(messages.GetMessage("stats_health") + HealthPoints);

        if (Weapon != null)
        {
            Console.WriteLine(messages.GetMessage("stats_weapon") + messages.TranslateWeaponForDisplay(Weapon.Type));
            Console.WriteLine(messages.GetMessage("stats_damage") + Weapon.MaxDamage);
        }

        Console.WriteLine(messages.GetMessage("stats_gold") + Gold);
        Console.WriteLine(messages.GetMessage("stats_armor") + GetArmorDisplay(messages));
        Console.WriteLine(messages.GetMessage("stats_potions") + HealingPotions);
    }

    public string GetArmorDisplay(Messages messages)
    {
        if (Armor == null)
            return messages.GetMessage("armor_none");
        return string.Format(messages.GetMessage("armor_display"),
            messages.TranslateArmorForDisplay(Armor.Type), Armor.Protection);
    }

    public int RollAttack()
    {
        return _die.Roll(20);
    }

    public bool AttackHits(int attackRoll)
    {
        return attackRoll <= Strength;
    }

    public int RollDamage()
    {
        if (Weapon == null)
            return 0;

        return _die.Roll(Weapon.MaxDamage);
    }

    public string Attack(IMonster dragon, Messages messages)
    {
        if (Weapon == null)
            return "You have no weapon to attack with.";

        var output = new List<string>();

        output.Add(string.Format(
            messages.GetMessage("player_attack_intro"),
            messages.TranslateOccupationForDisplay(Occupation),
            messages.TranslateWeaponForDisplay(Weapon.Type)
        ));

        // If this fails to compile, change AsciiArt to whatever exists in Weapon.cs
        output.Add(Weapon.AsciiArt);

        int attackRoll = RollAttack();

        // MISS
        if (!AttackHits(attackRoll))
        {
            output.Add(string.Format(messages.GetMessage("player_missed_dragon"), dragon.Name));
            return string.Join(Environment.NewLine, output);
        }

        output.Add(string.Format(messages.GetMessage("player_hit_dragon"), dragon.Name));

        int defenseRoll = _die.Roll(20);

        // DEFENSE
        if (defenseRoll <= dragon.Agility)
        {
            output.Add(string.Format(messages.GetMessage("dragon_defended_player_attack"), dragon.Name));
            output.Add(string.Format(
                dragon.GetRandomDefenseTaunt(messages),
                messages.TranslateRaceForDisplay(Race)
            ));
            return string.Join(Environment.NewLine, output);
        }

        // DAMAGE
        int damage = RollDamage();
        dragon.HealthPoints -= damage;

        output.Add(string.Format(messages.GetMessage("player_damage_dealt"), damage));
        output.Add(string.Format(messages.GetMessage("dragon_health_now"), dragon.Name, dragon.HealthPoints));

        // END
        if (dragon.HealthPoints <= 0)
            output.Add(dragon.GetDefeatedNarrative(messages));
        else
            output.Add(dragon.GetRandomDamageReply(messages));

        return string.Join(Environment.NewLine, output);
    }

    private static readonly Dictionary<string, string> RollOnly = new() { ["roll"] = "roll" };

    private void PromptRoll(string promptKey, Prompt prompt)
    {
        prompt.AskChoice(promptKey, RollOnly, "roll_invalid");
    }
}