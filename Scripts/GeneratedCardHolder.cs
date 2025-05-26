using System.Collections.Generic;

namespace RandomCardsGenerators {
    internal static class GeneratedCardHolder {
        public static readonly Dictionary<string, List<GeneratedCardInfo>> generatedCards = new Dictionary<string, List<GeneratedCardInfo>>();

        public static List<GeneratedCardInfo> GetGeneratedCards(string statGenName) {
            if(generatedCards.ContainsKey(statGenName)) {
                return generatedCards[statGenName];
            }
            return new List<GeneratedCardInfo>();
        }
        public static void AddCardToGenerated(string statGenName, GeneratedCardInfo cardInfo) {
            if(generatedCards.ContainsKey(statGenName)) {
                generatedCards[statGenName].Add(cardInfo);
            } else {
                generatedCards.Add(statGenName, new List<GeneratedCardInfo> { cardInfo });
            }
        }
    }
}