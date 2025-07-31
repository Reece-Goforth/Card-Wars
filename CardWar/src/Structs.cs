using System;

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
        private CardRarity id { get; set; }
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

        public CardStats(int Health, int Attack, int Defense, int Evasion)
        {
            this.Health = Health;
            this.Attack = Attack;
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
}
