public class Game
{
    public void StartGame()
    {
        // Title screen
        Console.WriteLine("=====================================");
        Console.WriteLine("Welcome to Choose Your Adventure Game");
        Console.WriteLine("=====================================\n");

        // Instantiate Die first (per sequence diagram)
        Die die = new Die();

        Dragon dragon = new Dragon(die);

        // Language selection shown before Messages dictionary is loaded
        Console.WriteLine("Select language / Selecciona idioma / Choisissez la langue / Seleziona la lingua:");
        Console.WriteLine("1. English  2. Espanol  3. Francais  4. Italiano");
        Console.Write("Enter choice: ");
        string? langInput = Console.ReadLine()?.Trim();
        string selectedLanguage = langInput switch { "2" => "Spanish", "3" => "French", "4" => "Italian", _ => "English" };

        // Initialize Messages with selected language (per sequence diagram)

        Messages messages = new Messages();
        messages.SetCurrentLanguage(selectedLanguage);
        messages.ReadDictionary();

        ProfileStore profiles = new ProfileStore(ProfileStore.DefaultDirectory);
        Prompt prompt = new Prompt(messages);

        // A missing store is the ordinary first run and says nothing. A damaged
        // one is set aside, reported, and never silently overwritten.
        if (profiles.SetAsideUnreadableStore())
            Console.WriteLine(messages.GetMessage("store_unreadable"));
        // Display welcome message
        Console.WriteLine();
        Console.WriteLine(messages.GetMessage("welcome"));
        Console.WriteLine();

        // Main menu loop
        bool running = true;
        while (running)
        {
            int choice = prompt.AskNumber("menu", 2, "invalid", "enter_choice");

            if (choice == 1)
            {
                Player player = StartOrResumeCharacter(profiles, messages, prompt);

                bool inAdventureMenu = true;
                while (inAdventureMenu)
                {
                    // Autosave: the adventure menu is a quiescent state, so no
                    // encounter is ever mid-flight when the Profile is written.
                    profiles.Save(player);

                    string selectedPath = PromptForPath(prompt);

                    if (selectedPath == "exit")
                    {
                        Console.WriteLine(messages.GetMessage("adventure_ends"));
                        running = false;
                        inAdventureMenu = false;
                    }
                    else if (selectedPath == "south")
                    {
                        Merchant merchant = new Merchant(die);
                        if (merchant.RollEncounter())
                            RunMerchantShop(player, merchant, messages, prompt);
                        else
                            Console.WriteLine(messages.GetMessage("south_path_narrative"));
                    }
                    else if (selectedPath == "east")
                    {
                        Wolf wolf = new Wolf(die);
                        if (!wolf.RollEncounter())
                        {
                            Console.WriteLine(messages.GetMessage("east_path_narrative"));
                        }
                        else if (!HandleWolfEncounter(player, wolf, messages, prompt))
                        {
                            // Unlike the dragon, only death ends the run here.
                            EndGame(false, false, messages);
                            running = false;
                            inAdventureMenu = false;
                        }
                    }
                    else if (selectedPath == "north")
                    {
                        HandleDragonEncounter(player, dragon, messages, prompt);

                        running = false;
                        inAdventureMenu = false;
                    }
                }
            }
            else
            {
                Console.WriteLine(messages.GetMessage("goodbye"));
                running = false;
            }
        }
    }

    // Offers Continue only when a Profile exists, so a first-time player sees
    // exactly the flow the game had before Profiles existed.
    private Player StartOrResumeCharacter(ProfileStore profiles, Messages messages, Prompt prompt)
    {
        List<string> names = profiles.ListNames();

        // 2 is Continue on the launch menu.
        if (names.Count > 0 && prompt.AskNumber("profile_menu", 2, "invalid", "enter_choice") == 2)
        {
            Player? resumed = PromptForProfile(profiles, names, messages, prompt);
            if (resumed != null)
            {
                Console.WriteLine(string.Format(messages.GetMessage("profile_resumed"), resumed.Name));
                return resumed;
            }
        }

        Player player = new Player();
        player.CreateCharacter(messages, prompt, profiles.Exists);
        return player;
    }

    private Player? PromptForProfile(ProfileStore profiles, List<string> names, Messages messages, Prompt prompt)
    {
        var lines = new List<string> { messages.GetMessage("profile_select_prompt") };
        for (int i = 0; i < names.Count; i++)
            lines.Add(string.Format(messages.GetMessage("profile_option"), i + 1, names[i]));

        int choice = prompt.AskNumber(lines, names.Count, "invalid", "enter_choice");
        return profiles.Load(names[choice - 1]);
    }

    public void EndGame(bool playerWon, bool playerRetreated, Messages messages)
    {
        if (playerRetreated)
            Console.WriteLine(messages.GetMessage("retreat"));
        else if (playerWon)
            Console.WriteLine(messages.GetMessage("victory"));
        else
            Console.WriteLine(messages.GetMessage("defeat"));
    }

    public bool IsValidMenuChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim();
        return trimmed == "1" || trimmed == "2";
    }

    public bool IsValidPathChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        // East takes the full word only: 'e' is already bound to exit.
        return trimmed == "n" || trimmed == "north" ||
               trimmed == "s" || trimmed == "south" ||
               trimmed == "east" ||
               trimmed == "e" || trimmed == "exit";
    }

    public static readonly Dictionary<string, string> AttackOrRetreat = new()
    {
        ["a"] = "attack",
        ["attack"] = "attack",
        ["r"] = "retreat",
        ["retreat"] = "retreat"
    };

    public bool IsValidCombatChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "a" || trimmed == "attack" ||
               trimmed == "r" || trimmed == "retreat";
    }

    // East is the full word only: 'e' is already bound to exit.
    private static readonly Dictionary<string, string> PathChoices = new()
    {
        ["n"] = "north",
        ["north"] = "north",
        ["s"] = "south",
        ["south"] = "south",
        ["east"] = "east",
        ["e"] = "exit",
        ["exit"] = "exit"
    };

    private string PromptForPath(Prompt prompt)
    {
        return prompt.AskChoice("path_prompt_full", PathChoices, "path_invalid", "enter_choice");
    }

    // Wandering Merchant shop. All shop rules live in Merchant; this loop
    // only reads input and prints localized text (see ADR-0002).
    private void RunMerchantShop(Player player, Merchant merchant, Messages messages, Prompt prompt)
    {
        Console.WriteLine(messages.GetMessage("merchant_appears"));
        Console.WriteLine(messages.GetMessage("merchant_greeting"));

        while (true)
        {
            // Menu numbers are assigned dynamically so only in-stock items appear.
            var actions = new List<string>();
            var lines = new List<string> { string.Format(messages.GetMessage("shop_gold"), player.Gold) };

            if (merchant.OffersEnchantedSword(player))
            {
                actions.Add("sword");
                lines.Add(string.Format(messages.GetMessage("shop_option_sword"), actions.Count));
            }
            if (merchant.OffersEnchantedArmor(player))
            {
                actions.Add("armor");
                lines.Add(string.Format(messages.GetMessage("shop_option_armor"), actions.Count));
            }
            if (merchant.CanSellWeapon(player))
            {
                actions.Add("sell");
                lines.Add(string.Format(
                    messages.GetMessage("shop_option_sell"),
                    actions.Count,
                    messages.TranslateWeaponForDisplay(player.Weapon!.Type),
                    player.Weapon.MaxDamage));
            }
            actions.Add("leave");
            lines.Add(string.Format(messages.GetMessage("shop_option_leave"), actions.Count));

            int choice = prompt.AskNumber(lines, actions.Count, "shop_invalid", "enter_choice");

            string action = actions[choice - 1];
            if (action == "leave")
            {
                Console.WriteLine(messages.GetMessage("merchant_farewell"));
                return;
            }

            if (action == "sell")
            {
                string soldWeapon = messages.TranslateWeaponForDisplay(player.Weapon!.Type);
                int credited = merchant.SellWeapon(player);
                Console.WriteLine(string.Format(messages.GetMessage("sell_success"), soldWeapon, credited));
                continue;
            }

            if (action == "sword")
            {
                // A weapon purchase discards the current weapon: warn first.
                string currentWeapon = messages.TranslateWeaponForDisplay(player.Weapon!.Type);
                if (!prompt.AskYesNo(new[] { string.Format(messages.GetMessage("weapon_discard_warning"), currentWeapon) }))
                {
                    Console.WriteLine(messages.GetMessage("buy_cancelled"));
                    continue;
                }
            }

            // Optional Haggle before paying: success takes 10 Gold off this
            // item; failure ends the entire encounter (nothing bought).
            int price = Merchant.Price;
            if (prompt.AskYesNo("haggle_prompt"))
            {
                bool bargainStruck = merchant.Haggle(player, out int roll);
                Console.WriteLine(string.Format(messages.GetMessage("haggle_roll"), roll, player.Agility));
                if (bargainStruck)
                {
                    price -= Merchant.HaggleDiscount;
                    Console.WriteLine(messages.GetMessage("haggle_success"));
                }
                else
                {
                    Console.WriteLine(messages.GetMessage("haggle_fail"));
                    return;
                }
            }

            if (action == "sword")
            {
                PurchaseOutcome outcome = merchant.BuyEnchantedSword(player, price);
                ReportPurchase(outcome, messages.TranslateWeaponForDisplay(Merchant.EnchantedSwordType), price, messages);
            }
            else
            {
                PurchaseOutcome outcome = merchant.BuyEnchantedArmor(player, price);
                ReportPurchase(outcome, messages.TranslateArmorForDisplay(Merchant.EnchantedArmorType), price, messages);
            }
        }
    }

    private void ReportPurchase(PurchaseOutcome outcome, string itemName, int price, Messages messages)
    {
        if (outcome == PurchaseOutcome.Purchased)
            Console.WriteLine(string.Format(messages.GetMessage("buy_success"), price, itemName));
        else if (outcome == PurchaseOutcome.InsufficientGold)
            Console.WriteLine(string.Format(messages.GetMessage("buy_insufficient"), price));
    }

    // Returns whether the player walks away. Winning and retreating both put them
    // back on the adventure menu carrying their wounds; only dying ends the run.
    private bool HandleWolfEncounter(Player player, Wolf wolf, Messages messages, Prompt prompt)
    {
        Console.WriteLine(messages.GetMessage("wolf_appears"));
        Console.WriteLine(messages.GetMessage("wolf_stats_intro"));
        wolf.DisplayStats(messages);

        if (prompt.AskChoice("dragon_encounter_prompt", AttackOrRetreat, "invalid", "enter_choice") == "attack")
        {
            new Combat(player, wolf, messages, prompt).StartCombat();
            return player.HealthPoints > 0;
        }

        // Not the dragon's retreat line: that one ends the run in its wording,
        // and backing away from a wolf sends you back to the menu.
        Console.WriteLine(messages.GetMessage("wolf_retreat"));
        return true;
    }

    private void HandleDragonEncounter(Player player, Dragon dragon, Messages messages, Prompt prompt)
    {
        Console.WriteLine(messages.GetMessage("north_path_narrative"));

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(string.Format(messages.GetMessage("dragon_intro"), dragon.Name));
        Console.ResetColor();

        Console.WriteLine(messages.GetMessage("dragon_stats_intro"));
        dragon.DisplayStats(messages);

        if (prompt.AskChoice("dragon_encounter_prompt", AttackOrRetreat, "invalid", "enter_choice") == "retreat")
        {
            Console.WriteLine(messages.GetMessage("retreat"));
            return;
        }

        Combat combat = new Combat(player, dragon, messages, prompt);
        bool playerWon = combat.StartCombat();

        // Combat already printed its own retreat message.
        if (!combat.PlayerRetreated)
            EndGame(playerWon, false, messages);
    }
}
