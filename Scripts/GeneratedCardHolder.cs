using System.Collections.Generic;
using System.Linq;

namespace RandomCardsGenerators {
    internal static class GeneratedCardHolder {
        public static readonly Dictionary<string, List<GeneratedCardInfo>> GeneratedCardsMap = new Dictionary<string, List<GeneratedCardInfo>>();
        public static readonly List<GeneratedCardInfo> GeneratedCards = new List<GeneratedCardInfo>();

        public static List<GeneratedCardInfo> GetGeneratedCards(string statGenName) {
            if(GeneratedCardsMap.ContainsKey(statGenName)) {
                return GeneratedCardsMap[statGenName];
            }
            return new List<GeneratedCardInfo>();
        }
        public static GeneratedCardInfo IsCardGenerated(CardInfo cardInfo) {
            return GeneratedCards.FirstOrDefault(c => c.CardInfo == cardInfo);
        }

        public static void AddCardToGenerated(string statGenName, GeneratedCardInfo cardInfo) {
            if(GeneratedCardsMap.ContainsKey(statGenName)) {
                GeneratedCardsMap[statGenName].Add(cardInfo);
            } else {
                GeneratedCardsMap.Add(statGenName, new List<GeneratedCardInfo> { cardInfo });
            }
            GeneratedCards.Add(cardInfo);
        }
    }
}