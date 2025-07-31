using System;
using Newtonsoft.Json;

namespace CardWar
{
    class Card
    {
        [JsonRequired]
        private int Id { get; set; }
        [JsonRequired]
        private int Level { get; set; }
        [JsonRequired]
        private string Name { get; set; }
        [JsonRequired]
        private string Description { get; set; }
        [JsonRequired]
        private RarityData Rarity { get; set; }
        [JsonRequired]
        private CardStats Stats { get; set; }

        public Card(string name, string desc, RarityData rarity, CardStats stats)
        {
            Name = name;
            Description = desc;
            Rarity = rarity;
            Stats = stats;
            Level = 1;
        }
        public Card()
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
        public int GetLevel() { return Level; }
        public void LevelUp(int hpIncrease, int atkIncrease, int defIncrease, int evadeIncrease)
        {
            Level++;
            Stats = new CardStats(
                Stats.Health  + hpIncrease,
                Stats.Attack + atkIncrease,
                Stats.Defense  + defIncrease,
                Stats.Evasion + evadeIncrease
            );

        }
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
