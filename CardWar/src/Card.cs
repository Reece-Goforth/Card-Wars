using System;
using Newtonsoft.Json;

namespace CardWar
{
    class Card
    {
        public int Id { get; private set; }
        public int Level { get; private set; }
        [JsonRequired]
        public string Name { get; private set; }
        [JsonRequired]
        public string Description { get; private set; }
        [JsonRequired]
        private RarityData Rarity { get; set; }
        [JsonRequired]
        public CardStats Stats { get; private set; }

        public Card()
        {
            Level = 1;
        }
        public Card(string name, string desc, RarityData rarity, CardStats stats)
        {
            Level = 1;
            Name = name;
            Description = desc;
            Rarity = rarity;
            Stats = stats;
        }

        public override string ToString()
        {
            return (
                $"[{GetRarityName()}]   [{Name}: {Description}]   [{Stats}]"
            );
        }
        public int GetId() { return Id; }
        public void SetId(int Id) { this.Id = Id; }
        public void SetName(string Name) { this.Name = Name.Substring(0, Math.Min(20, Name.Length)); }
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
        public void LevelUp(int hpIncrease, int atkIncrease, int defIncrease, int evadeIncrease)
        {
            Level++;
            Stats = new CardStats(
                Stats.Health + hpIncrease,
                Stats.Attack + atkIncrease,
                Stats.Defense + defIncrease,
                Stats.Evasion + evadeIncrease
            );

        }
    }
}
