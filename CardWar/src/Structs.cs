using System;
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
        public CardRarity Id { get; set; }
        public ConsoleColor Color { get; set; }
        public string Name { get; set; }

        public RarityData(CardRarity Id)
        {
            // Default case
            Color = ConsoleColor.White;
            Name = "NONE";
            
            switch (Id)
            {
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
