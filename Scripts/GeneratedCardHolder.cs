using System.Collections.Generic;

namespace RandomCardsGenerators {
    internal static class GeneratedCardHolder {
        public static readonly Dictionary<string, List<CardInfo>> generatedCards = new Dictionary<string, List<CardInfo>>();

        public static List<CardInfo> GetGeneratedCards(string statGenName) {
            if(generatedCards.ContainsKey(statGenName)) {
                return generatedCards[statGenName];
            }
            return new List<CardInfo>();
        }
        public static void AddCardToGenerated(string statGenName, CardInfo cardInfo) {
            if(generatedCards.ContainsKey(statGenName)) {
                generatedCards[statGenName].Add(cardInfo);
            } else {
                generatedCards.Add(statGenName, new List<CardInfo> { cardInfo });
            }
        }
    }
}