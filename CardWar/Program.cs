using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
        private CardRarity id;
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
        public ConsoleColor Color;
        public string Name;

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

    internal class Program
    {
        public static readonly ConsoleColor DefaultColor = ConsoleColor.White;

        public static int Money = 2000;
        static void DrawMenu()
        {
            Console.ForegroundColor = DefaultColor;
            Console.WriteLine("\n[MAIN MENU]");
            DrawMoneyCount();
            Console.WriteLine("0) Exit\n1) Create card\n2) List deck\n");
            Console.Write("Enter > ");
        }
        static void DrawMoneyCount()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nMONEY: {Money}\n");
            Console.ForegroundColor = DefaultColor;
        }
        static void Main(string[] args)
        {
            Deck deck = new Deck(50);

            DrawMenu();
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
                    deck.ListCards();
                }
                DrawMenu();
                
                key = Console.ReadKey();
            }
            Environment.Exit(0);
        }

        static bool AddNewCardToDeck(Deck deck)
        {
            if (!deck.RoomInDeck()) 
            {
                Console.WriteLine("\nCreate Card failed: No room in deck...");
                return false;
            }

            Card newCard = new Card();

            Console.Clear();
        SettingRarity:
            DrawMoneyCount();
            Console.WriteLine("Choose Rarity:\n1) Common [$100],\n2) Uncommon [$200]," +
                "\n3) Rare [$300],\n4) Epic [$400],\n5) Ledgendary [$500]\n6) Mythic [$600]" +
                "\nB) back to menu");
            ConsoleKeyInfo key = Console.ReadKey();
            string chr = "" + key.KeyChar;

            if (chr.ToUpper().Equals("B")) 
            {
                Console.Clear();
                return false; 
            }

            int chrNum;
            if (!int.TryParse(chr, out chrNum) || chrNum < 1 || chrNum > 6)
            {
                Console.WriteLine(" is invalid. Must be a number from 1-6...");
                goto SettingRarity;
            }
            if ((chrNum * 100) > Program.Money)
            {
                Console.WriteLine("\nNot enough funds! Pick something else or return to menu (B)...");
                goto SettingRarity;
            }
            newCard.SetCardRarity((CardRarity)chrNum);

            Console.Clear();
            Console.Write("Name: ");
            string input = Console.ReadLine();
            while (input == "")
            {
                Console.Write("Name cannot be empty!\n");
                Console.Write("Name: ");
                input = Console.ReadLine();
            }
            newCard.SetName(input);

            Console.Clear();
            Console.Write("Description: ");
            input = Console.ReadLine();
            while (input == "")
            {
                Console.Write("Description cannot be empty!\n");
                Console.Write("Description: ");
                input = Console.ReadLine();
            }
            newCard.SetDescription(input);
            Console.Clear();

            deck.AddCard(newCard);
            Program.Money -= chrNum*100;
            Console.WriteLine("Card added to deck...");
            return true;
        }
    }

    class Deck
    {
        private List<Card> cards = new List<Card>();
        private readonly int maxSize;

        public Deck(int maxSize)
        {
            this.maxSize = maxSize;
        }

        public void ListCards()
        {
            Console.Write("\nList of cards in deck:");
            Console.WriteLine($" (Available slots: {maxSize - GetCardCount()})\n");
            if (cards == null || cards.Count == 0)
            {
                Console.WriteLine("\tThis deck is empty...");
                return;
            }
            SortDeck();
            foreach (Card card in  cards)
            {
                Console.ForegroundColor = card.GetRarityColor();
                Console.WriteLine($"\t{card}");
                Console.ForegroundColor = Program.DefaultColor;
            }
        }
        public void SortDeck()
        {
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

    class Card
    {
        private int Id { get; set; }
        private string Name { get; set; }
        private string Description { get; set; }
        private RarityData Rarity {  get; set; }

        public Card(string name, string desc, RarityData rarity)
        {
            Name = name;
            Description = desc;
            Rarity = rarity;
        }
        public Card ()
        {
        }

        public override string ToString()
        {
            return (
                $"[{GetRarityName()}] {Name}: {Description}"
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
    }
}
