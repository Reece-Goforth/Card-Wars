using System;
using System.IO;
using Newtonsoft.Json;

namespace CardWar
{
    internal class Program
    {
        public static readonly ConsoleColor DefaultColor = ConsoleColor.White;

        static void DrawMenu(Deck currentDeck)
        {
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine("\n[MAIN MENU]");
            DrawMoneyCount(currentDeck);
            Console.WriteLine("0) Exit\n1) Create card\n2) List deck\n3) Save deck\n4) Load deck\n");
            Console.Write("Enter > ");
        }
        static void DrawMoneyCount(Deck currentDeck)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nMONEY: {currentDeck.Money}\n");
            Console.ForegroundColor = DefaultColor;
        }
        public static void WriteLineColor(string str, ConsoleColor col)
        {
            Console.ForegroundColor = col;
            Console.WriteLine(str);
            Console.ForegroundColor = DefaultColor;
        }
        public static void WriteColor(string str, ConsoleColor col)
        {
            Console.ForegroundColor = col;
            Console.Write(str);
            Console.ForegroundColor = DefaultColor;
        }
        static void Main()
        {
            Deck deck = new Deck(50);

            Directory.CreateDirectory(Environment.CurrentDirectory + "\\SavedDecks");
            string filePath = Environment.CurrentDirectory + "\\SavedDecks\\";

            DrawMenu(deck);
            Console.Write("Enter > ");
            ConsoleKeyInfo key = Console.ReadKey();
            while (key.KeyChar != '0')
            {
                if (key.KeyChar == '1')
                {
                    if (AddNewCardToDeck(deck))
                    {
                        deck.ListCards();
                    }
                }
                if (key.KeyChar == '2')
                {
                    Console.Clear();
                    deck.ListCards();
                }
                if (key.KeyChar == '3')
                {
                    Console.Clear();

                    Console.WriteLine("What to name this deck?\n");
                    Console.Write("Enter > ");
                    string fileName = Console.ReadLine() + ".json";

                    var deckJson = JsonConvert.SerializeObject(deck, Formatting.Indented);
                    File.WriteAllText(filePath + fileName, deckJson);
                    WriteLineColor($"Successfully saved deck to: {filePath}", ConsoleColor.Green);
                }
                if (key.KeyChar == '4')
                {
                    Console.Clear();
                    Console.WriteLine("Choose a deck from the list:");
                    string[] files = Directory.GetFiles(filePath);

                    foreach (string file in files)
                    {
                        var str = file.Replace(filePath, "").Replace(".json", "");
                        WriteLineColor($"\t[{str}]", ConsoleColor.DarkGray);
                    }
                    Console.WriteLine();
                    Console.Write("Enter > ");
                    string fileName = Console.ReadLine() + ".json";
                    while (!File.Exists(filePath + fileName))
                    {
                        WriteLineColor("File does not exist!", ConsoleColor.DarkRed);
                        Console.WriteLine();
                        Console.Write("Enter > ");
                        fileName = Console.ReadLine() + ".json";
                    }

                    string deckJson = File.ReadAllText(filePath + fileName);
                    deck = JsonConvert.DeserializeObject<Deck>(deckJson);
                    WriteLineColor("Loaded deck successfully...", ConsoleColor.Green);
                }
                DrawMenu(deck);

                key = Console.ReadKey();
            }
            Environment.Exit(0);
        }

        static bool AddNewCardToDeck(Deck deck)
        {
            if (!deck.RoomInDeck())
            {
                WriteLineColor("\nCreate Card failed: No room in deck...", ConsoleColor.DarkRed);
                return false;
            }

            Card newCard = new Card();

            Console.Clear();
        SettingRarity:
            // Rarity Menu
            DrawMoneyCount(deck);
            Console.WriteLine("Choose Rarity:\n1) Common [$100],\n2) Uncommon [$200]," +
                "\n3) Rare [$300],\n4) Epic [$400],\n5) Ledgendary [$500]\n6) Mythic [$600]" +
                "\nB) back to menu\n");
            Console.Write("Enter > ");
            ConsoleKeyInfo key = Console.ReadKey();
            string chr = "" + key.KeyChar;

            if (chr.ToUpper().Equals("B"))
            {
                Console.Clear();
                return false;
            }
            // Extract input character as a number, and perform the necesarry operations
            if (!int.TryParse(chr, out int chrNum) || chrNum < 1 || chrNum > 6)
            {
                WriteLineColor(" [INVALID] Must be a number from 1-6...", ConsoleColor.DarkRed);
                goto SettingRarity;
            }
            if ((chrNum * 100) > deck.Money)
            {
                WriteLineColor("\nNot enough funds! Pick something else or return to menu (B)...", ConsoleColor.DarkRed);
                goto SettingRarity;
            }
            newCard.SetCardRarity((CardRarity)chrNum);

            // Set Name of Card
            Console.Clear();
            Console.Write("Name: ");
            string input = Console.ReadLine();
            while (input == "")
            {
                WriteColor("Name cannot be empty!\n", ConsoleColor.DarkRed);
                Console.Write("Name: ");
                input = Console.ReadLine();
            }
            newCard.SetName(input);

            // Set DESC of card
            Console.Clear();
            Console.Write("Description: ");
            input = Console.ReadLine();
            while (input == "")
            {
                WriteColor("Description cannot be empty!\n", ConsoleColor.DarkRed);
                Console.Write("Description: ");
                input = Console.ReadLine();
            }
            newCard.SetDescription(input);
            Console.Clear();

            // Menu for adding stats
            int maxPoints = chrNum * 100;
            CardStats newStats = new CardStats();

        SettingStats:
            Console.WriteLine("[STAT ALLOCATION]\n");
            Console.WriteLine($"Points used: {newStats.GetTotalValue()}, " +
                $"Points left: {maxPoints - newStats.GetTotalValue()}\n");
            Console.WriteLine("Current Stats:");
            WriteLineColor(newStats.ToString(), newCard.GetRarityColor());
            Console.WriteLine("\nSelect a stat to modify:\n1) Health (HP)\n2) Attack (ATK)" +
                "\n3) Defense (DEF)\n4) Evasion (EVADE)\n5) Done");
            Console.Write("Enter > ");

            key = Console.ReadKey();

            while (!char.IsDigit(key.KeyChar))
            {
                Console.Clear();
                WriteLineColor("Invalid input. Type a number from 1-5\n", ConsoleColor.DarkRed);
                goto SettingStats;
            }

            switch (key.KeyChar)
            {
                default:
                    Console.Clear();
                    WriteLineColor("Invalid input. Type a number from 1-5\n", ConsoleColor.DarkRed);
                    goto SettingStats;
                case '1':
                    newStats.Health = 0;
                    int maxVal = maxPoints - newStats.GetTotalValue();
                    Console.Clear();
                    Console.WriteLine($"\nSet Health Points (0-{maxVal}):\n");
                    Console.Write("Enter > ");
                    // Get inital input
                    bool inputIsInt = int.TryParse(Console.ReadLine(), out int intEntered);

                    while (!inputIsInt || intEntered < 0 || intEntered > maxVal)
                    {
                        WriteLineColor($"Invalid input, must be a positive integer between 0-{maxVal}.\n", ConsoleColor.DarkRed);
                        Console.Write("Enter > ");

                        inputIsInt = int.TryParse(Console.ReadLine(), out intEntered);
                    }
                    // Successful entry
                    newStats.Health = intEntered;
                    Console.Clear();
                    goto SettingStats;
                case '2':
                    newStats.Attack = 0;
                    maxVal = maxPoints - newStats.GetTotalValue();
                    Console.Clear();
                    Console.WriteLine($"\nSet Attack Points (0-{maxVal}):\n");
                    Console.Write("Enter > ");
                    // Get inital input
                    inputIsInt = int.TryParse(Console.ReadLine(), out intEntered);

                    while (!inputIsInt || intEntered < 0 || intEntered > maxVal)
                    {
                        WriteLineColor($"Invalid input, must be a positive integer between 0-{maxVal}.\n", ConsoleColor.DarkRed);
                        Console.Write("Enter > ");

                        inputIsInt = int.TryParse(Console.ReadLine(), out intEntered);
                    }
                    // Successful entry
                    newStats.Attack = intEntered;
                    Console.Clear();
                    goto SettingStats;
                case '3':
                    newStats.Defense = 0;
                    maxVal = maxPoints - newStats.GetTotalValue();
                    Console.Clear();
                    Console.WriteLine($"\nSet Defense Points (0-{maxVal}):\n");
                    Console.Write("Enter > ");
                    // Get inital input
                    inputIsInt = int.TryParse(Console.ReadLine(), out intEntered);

                    while (!inputIsInt || intEntered < 0 || intEntered > maxVal)
                    {
                        WriteLineColor($"Invalid input, must be a positive integer between 0-{maxVal}.\n", ConsoleColor.DarkRed);
                        Console.Write("Enter > ");

                        inputIsInt = int.TryParse(Console.ReadLine(), out intEntered);
                    }
                    // Successful entry
                    newStats.Defense = intEntered;
                    Console.Clear();
                    goto SettingStats;
                case '4':
                    newStats.Evasion = 0;
                    maxVal = maxPoints - newStats.GetTotalValue();
                    Console.Clear();
                    Console.WriteLine($"\nSet Evasion Points (1-{maxVal}):\n");
                    Console.Write("Enter > ");
                    // Get inital input
                    inputIsInt = int.TryParse(Console.ReadLine(), out intEntered);

                    while (!inputIsInt || intEntered < 0 || intEntered > maxVal)
                    {
                        WriteLineColor($"Invalid input, must be a positive integer between 0-{maxVal}.\n", ConsoleColor.DarkRed);
                        Console.Write("Enter > ");

                        inputIsInt = int.TryParse(Console.ReadLine(), out intEntered);
                    }
                    // Successful entry
                    newStats.Evasion = intEntered;
                    Console.Clear();
                    goto SettingStats;
                case '5':
                    if (newStats.GetTotalValue() < maxPoints)
                    {
                        Console.Clear();
                        WriteLineColor($"You still have {maxPoints - newStats.GetTotalValue()} " +
                            $"unallocated points. Use all of them.\n", ConsoleColor.DarkRed);
                        goto SettingStats;
                    }
                    if (newStats.Health <= 0)
                    {
                        Console.Clear();
                        WriteLineColor($"Your card must have at least 1 HP", ConsoleColor.DarkRed);
                        goto SettingStats;
                    }
                    goto AddingCard;
            }
        AddingCard:
            Console.Clear();
            newCard.SetStats(newStats);
            deck.AddCard(newCard);
            deck.Money -= maxPoints;
            WriteLineColor("Card added to deck...", newCard.GetRarityColor());
            return true;
        }
    }
}
