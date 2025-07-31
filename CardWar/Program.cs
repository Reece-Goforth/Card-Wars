using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CardWar
{
    enum CardRarity
    {
        None,
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Mythic,
    }

    struct RarityData
    {
        private CardRarity id {  get; set; }
        public CardRarity Id
        {
            get { return id; }
            set
            {
                id = value;
                switch (value)
                {
                    default:
                    case CardRarity.None:
                        break;
                    case CardRarity.Common:
                        Color = ConsoleColor.DarkGray;
                        Name = "COMMON";
                        break;
                    case CardRarity.Uncommon:
                        Color = ConsoleColor.DarkGreen;
                        Name = "UNCOMMON";
                        break;
                    case CardRarity.Rare:
                        Color = ConsoleColor.Blue;
                        Name = "RARE";
                        break;
                    case CardRarity.Epic:
                        Color = ConsoleColor.DarkMagenta;
                        Name = "EPIC";
                        break;
                    case CardRarity.Legendary:
                        Color = ConsoleColor.DarkYellow;
                        Name = "LEGENDARY";
                        break;
                    case CardRarity.Mythic:
                        Color = ConsoleColor.Red;
                        Name = "MYTHIC";
                        break;
                }
            }
        }
        public ConsoleColor Color { get; set; }
        public string Name { get; set; }

        public RarityData(CardRarity Id)
        {
            Color = ConsoleColor.White;
            Name = "NONE";
            id = CardRarity.None;
            this.Id = Id;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    struct CardStats
    {
        public int Attack;
        public int Health;
        public int Defense;
        public int Evasion;

        public CardStats(int Attack, int Health, int Defense, int Evasion)
        {
            this.Attack = Attack;
            this.Health = Health;
            this.Defense = Defense;
            this.Evasion = Evasion;
        }
        public override string ToString()
        {
            return $"HP: {Health}, ATK: {Attack}, DEF:  {Defense}, EVADE: {Evasion}";
        }
        public int GetTotalValue()
        {
            return Attack + Health + Defense + Evasion;
        }
    }

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
                    while (!File.Exists(filePath+fileName))
                    {
                        WriteLineColor("File does not exist!", ConsoleColor.DarkRed);
                        Console.WriteLine();
                        Console.Write("Enter > ");
                        fileName = Console.ReadLine() + ".json";
                    }

                    string deckJson = File.ReadAllText(filePath+fileName);
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
                $"Points left: {maxPoints-newStats.GetTotalValue()}\n");
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

            switch(key.KeyChar)
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
                            $"unallocated points. Use all of them.\n",  ConsoleColor.DarkRed);
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

    class Deck
    {
        [JsonRequired]
        private List<Card> cards { get; set; }
        [JsonRequired]
        private readonly int maxSize;
        public int Money = 2000;

        public Deck(int maxSize)
        {
            this.maxSize = maxSize;
            cards = new List<Card>();
        }

        public void ListCards()
        {
            Console.Write("\nList of cards in deck:");
            Console.WriteLine($" (Available slots: {maxSize - GetCardCount()}/{maxSize})\n");
            if (cards == null || cards.Count == 0)
            {
                Console.WriteLine("\tThis deck is empty...");
                return;
            }
            SortDeck();
            foreach (Card card in  cards)
            {
                Program.WriteLineColor($"\t{card}", card.GetRarityColor());
            }
        }
        public void SortDeck()
        {
            // Sort by Rarity
            cards = cards.OrderBy(c => c.GetCardRarity()).ToList();
        }
        public int GetCardCount()
        {
            if (cards == null)
            {
                return 0;
            }
            return cards.Count;
        }
        public bool RoomInDeck()
        {
            return (GetCardCount() < maxSize);
        }
        public void AddCard(Card newCard)
        {
            newCard.SetId(GetCardCount());
            cards.Add(newCard);
        }
    }
    class foo
    {
        public int x { get; set; }
        public int y { get; set; }

        public foo(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class Card
    {
        [JsonRequired]
        private int Id { get; set; }
        [JsonRequired]
        private string Name { get; set; }
        [JsonRequired]
        private string Description { get; set; }
        [JsonRequired]
        private RarityData Rarity {  get; set; }
        [JsonRequired]
        private CardStats Stats { get; set; }

        public Card(string name, string desc, RarityData rarity, CardStats stats)
        {
            Name = name;
            Description = desc;
            Rarity = rarity;
            Stats = stats;
        }
        public Card ()
        {
        }

        public override string ToString()
        {
            return (
                $"[{GetRarityName()}]   [{Name}: {Description}]   [{Stats}]"
            );
        }
        public int GetId() { return Id; }
        public void SetId(int Id) { this.Id = Id; }
        public string GetName() { return Name; }
        public void SetName(string Name) { this.Name = Name.Substring(0, Math.Min(20, Name.Length)); }
        public string GetDescription() { return Description; }
        public void SetDescription(string Description) { this.Description = Description; }
        public CardRarity GetCardRarity() { return Rarity.Id; }
        public void SetCardRarity(CardRarity rarity) { Rarity = new RarityData(rarity); }
        public ConsoleColor GetRarityColor()
        {
            return Rarity.Color;
        }
        public string GetRarityName()
        {
            return Rarity.Name;
        }
        public void SetStats(CardStats stats)
        {
            Stats = stats;
        }
        public CardStats GetStats()
        {
            return Stats;
        }
    }
}
