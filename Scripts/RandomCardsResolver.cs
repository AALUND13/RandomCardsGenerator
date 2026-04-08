using PickPhaseImprovements;
using RandomCardsGenerators.Cards;
using RandomCardsGenerators.Utils;
using UnboundLib;

namespace RandomCardsGenerators {
    internal static class RandomCardsResolver {
        internal static CardInfo[] ResolveRandomCards(CardInfo[] hand) {
            Player player = (((PickerType)CardChoice.instance.GetFieldValue("pickerType") != 0)
                ? PlayerManager.instance.players[CardChoice.instance.pickrID]
                : PlayerManager.instance.GetPlayersInTeam(CardChoice.instance.pickrID)[0]);

            for(int i = 0; i < hand.Length; i++) {
                RandomCard randomCard = hand[i].GetComponent<RandomCard>();
                if(randomCard != null) {
                    bool doesGeneratorExist = RandomCardsGenerator.RandomStatCardGenerators.TryGetValue(randomCard.StatGenName, out var generator);
                    if(doesGeneratorExist) {
                        hand[i] = generator.CreateRandomCardForOther(player).GetComponent<CardInfo>();
                        PickManager.RegisterAlternetSpawnName(hand[i], $"__{generator.RandomCardOption.ModInitials}__{hand[i].cardName}");
                    } else {
                        LoggerUtils.LogError($"Stat generator {randomCard.StatGenName} does not exist.");
                    }
                }
            }
            return hand;
        }
    }
}
