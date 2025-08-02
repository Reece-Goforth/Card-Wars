using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CardWar
{
    class Deck
    {
        [JsonRequired]
        private List<Card> Cards { get; set; }
        [JsonRequired]
        private readonly int maxSize;
        public int Money = 2000;

        public Deck(int maxSize)
        {
            this.maxSize = maxSize;
            Cards = new List<Card>();
        }

        public void ListCards()
        {
            Console.Write("\nList of cards in deck:");
            Console.WriteLine($" (Available slots: {maxSize - GetCardCount()}/{maxSize})\n");
            if (Cards == null || Cards.Count == 0)
            {
                Console.WriteLine("\tThis deck is empty...");
                return;
            }
            SortDeck();
            foreach (Card card in Cards)
            {
                Program.WriteLineColor($"\t{card}", card.GetRarityColor());
            }
        }
        public void SortDeck()
        {
            // Sort by Rarity
            Cards = Cards.OrderBy(c => c.GetCardRarity()).ToList();
            Cards.Reverse();
        }
        public int GetCardCount()
        {
            if (Cards == null)
            {
                return 0;
            }
            return Cards.Count;
        }
        public bool RoomInDeck()
        {
            return (GetCardCount() < maxSize);
        }
        public void AddCard(Card newCard)
        {
            newCard.SetId(GetCardCount());
            Cards.Add(newCard);
        }
    }
}
