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
            Console.WriteLine(messages.GetMessage("menu"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? choice = Console.ReadLine()?.Trim();

            if (choice == "1")
            {
                Player player = StartOrResumeCharacter(profiles, messages);

                bool inAdventureMenu = true;
                while (inAdventureMenu)
                {
                    // Autosave: the adventure menu is a quiescent state, so no
                    // encounter is ever mid-flight when the Profile is written.
                    profiles.Save(player);

                    string selectedPath = PromptForPath(messages);

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
                            RunMerchantShop(player, merchant, messages);
                        else
                            Console.WriteLine(messages.GetMessage("south_path_narrative"));
                    }
                    else if (selectedPath == "north")
                    {
                        HandleDragonEncounter(player, dragon, messages);

                        running = false;
                        inAdventureMenu = false;
                    }
                }
            }
            else if (choice == "2")
            {
                Console.WriteLine(messages.GetMessage("goodbye"));
                running = false;
            }
            else
            {
                Console.WriteLine(messages.GetMessage("invalid"));
            }
        }
    }

    // Offers Continue only when a Profile exists, so a first-time player sees
    // exactly the flow the game had before Profiles existed.
    private Player StartOrResumeCharacter(ProfileStore profiles, Messages messages)
    {
        List<string> names = profiles.ListNames();

        if (names.Count > 0 && PlayerChoseContinue(messages))
        {
            Player? resumed = PromptForProfile(profiles, names, messages);
            if (resumed != null)
            {
                Console.WriteLine(string.Format(messages.GetMessage("profile_resumed"), resumed.Name));
                return resumed;
            }
        }

        Player player = new Player();
        player.CreateCharacter(messages, profiles.Exists);
        return player;
    }

    private bool PlayerChoseContinue(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("profile_menu"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim();

            if (input == "1") return false;
            if (input == "2") return true;

            Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    private Player? PromptForProfile(ProfileStore profiles, List<string> names, Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("profile_select_prompt"));
            for (int i = 0; i < names.Count; i++)
                Console.WriteLine(string.Format(messages.GetMessage("profile_option"), i + 1, names[i]));

            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim();

            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= names.Count)
                return profiles.Load(names[choice - 1]);

            Console.WriteLine(messages.GetMessage("invalid"));
        }
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
        return trimmed == "n" || trimmed == "north" ||
               trimmed == "s" || trimmed == "south" ||
               trimmed == "e" || trimmed == "exit";
    }

    public bool IsValidCombatChoice(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        string trimmed = input.Trim().ToLower();
        return trimmed == "a" || trimmed == "attack" ||
               trimmed == "r" || trimmed == "retreat";
    }

    private string PromptForPath(Messages messages)
    {
        while (true)
        {
            Console.WriteLine(messages.GetMessage("path_prompt_full"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "north" || input == "n")
                return "north";

            if (input == "south" || input == "s")
                return "south";

            if (input == "exit" || input == "e")
                return "exit";

            Console.WriteLine(messages.GetMessage("path_invalid"));
        }
    }

    // Wandering Merchant shop. All shop rules live in Merchant; this loop
    // only reads input and prints localized text (see ADR-0002).
    private void RunMerchantShop(Player player, Merchant merchant, Messages messages)
    {
        Console.WriteLine(messages.GetMessage("merchant_appears"));
        Console.WriteLine(messages.GetMessage("merchant_greeting"));

        while (true)
        {
            Console.WriteLine(string.Format(messages.GetMessage("shop_gold"), player.Gold));

            // Menu numbers are assigned dynamically so only in-stock items appear.
            var actions = new List<string>();
            if (merchant.OffersEnchantedSword(player))
            {
                actions.Add("sword");
                Console.WriteLine(string.Format(messages.GetMessage("shop_option_sword"), actions.Count));
            }
            if (merchant.OffersEnchantedArmor(player))
            {
                actions.Add("armor");
                Console.WriteLine(string.Format(messages.GetMessage("shop_option_armor"), actions.Count));
            }
            if (merchant.CanSellWeapon(player))
            {
                actions.Add("sell");
                Console.WriteLine(string.Format(
                    messages.GetMessage("shop_option_sell"),
                    actions.Count,
                    messages.TranslateWeaponForDisplay(player.Weapon!.Type),
                    player.Weapon.MaxDamage));
            }
            actions.Add("leave");
            Console.WriteLine(string.Format(messages.GetMessage("shop_option_leave"), actions.Count));

            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim();
            if (!int.TryParse(input, out int choice) || choice < 1 || choice > actions.Count)
            {
                Console.WriteLine(messages.GetMessage("shop_invalid"));
                continue;
            }

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
                if (!AskYesNo(string.Format(messages.GetMessage("weapon_discard_warning"), currentWeapon), messages))
                {
                    Console.WriteLine(messages.GetMessage("buy_cancelled"));
                    continue;
                }
            }

            // Optional Haggle before paying: success takes 10 Gold off this
            // item; failure ends the entire encounter (nothing bought).
            int price = Merchant.Price;
            if (AskYesNo(messages.GetMessage("haggle_prompt"), messages))
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

    private bool AskYesNo(string prompt, Messages messages)
    {
        while (true)
        {
            Console.WriteLine(prompt);
            string? answer = Console.ReadLine()?.Trim().ToLower();
            if (answer == "y") return true;
            if (answer == "n") return false;
            Console.WriteLine(messages.GetMessage("invalid"));
        }
    }

    private void ReportPurchase(PurchaseOutcome outcome, string itemName, int price, Messages messages)
    {
        if (outcome == PurchaseOutcome.Purchased)
            Console.WriteLine(string.Format(messages.GetMessage("buy_success"), price, itemName));
        else if (outcome == PurchaseOutcome.InsufficientGold)
            Console.WriteLine(string.Format(messages.GetMessage("buy_insufficient"), price));
    }

    private void HandleDragonEncounter(Player player, Dragon dragon, Messages messages)
    {
        Console.WriteLine(messages.GetMessage("north_path_narrative"));

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(string.Format(messages.GetMessage("dragon_intro"), dragon.Name));
        Console.ResetColor();

        Console.WriteLine(messages.GetMessage("dragon_stats_intro"));
        dragon.DisplayStats(messages);
        while (true)
        {
            Console.WriteLine(messages.GetMessage("dragon_encounter_prompt"));
            Console.Write(messages.GetMessage("enter_choice"));
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "a" || input == "attack")
            {
                Combat combat = new Combat(player, dragon, messages);
                bool playerWon = combat.StartCombat();

                if (combat.PlayerRetreated)
                {
                    return; //combat already handled retreat message
                }
                else
                {
                   EndGame(playerWon, false, messages); 
                }
                
                return;
            }

        if (input == "r" || input == "retreat")
        {
            Console.WriteLine(messages.GetMessage("retreat"));
            return;
        }

        Console.WriteLine(messages.GetMessage("invalid"));
    }
}
}
