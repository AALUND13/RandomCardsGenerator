using Photon.Pun;
using RandomCardsGenerators.Utils;
using UnboundLib;
using UnityEngine;

namespace RandomCardsGenerators.Cards {
    /// <summary>
    /// By default, just by creating a new instance of this class will <strong>NOT</strong> make the random card appear in hands.
    /// <para>You have to implement that functionality yourself. Take a look at [Corrupted Cards Manager Link Here] for an example.</para>
    /// </summary>
    public class DrawableRandomStatsCard {
        private static readonly System.Random random = new System.Random();

        public readonly RandomCardsGenerator StatCardGenerator;
        public readonly GameObject CardGameObject;
        public readonly CardInfo CardInfo;

        public DrawableRandomStatsCard(RandomCardsGenerator statCardGenerator) {
            GameObject cardGameObject = GameObject.Instantiate(Main.blankCardPrefab);
            cardGameObject.name = $"__{statCardGenerator.randomCardInfo.ModInitials}__{statCardGenerator.CardGenName}_DrawableCard";

            GameObject.Destroy(cardGameObject.transform.GetChild(0).gameObject);
            GameObject.DontDestroyOnLoad(cardGameObject);

            var card = cardGameObject.AddComponent<RandomStatsCard>();
            CardInfo = cardGameObject.GetComponent<CardInfo>();

            card.StatGenName = statCardGenerator.CardGenName;

            PhotonNetwork.PrefabPool.RegisterPrefab(cardGameObject.name, cardGameObject);

            CardGameObject = cardGameObject;
            StatCardGenerator = statCardGenerator;
        }

        public GameObject InstantiateCard(Vector3 position, Quaternion rotation, int seed, Vector3 localScale) {
            return PhotonNetwork.Instantiate(
                CardGameObject.name,
                position,
                rotation,
                0,
                new object[] { seed, localScale }
            );
        }
        public GameObject ReplaceCard(CardInfo cardInfo) {
            Main.instance.ExecuteAfterFrames(3, () => PhotonNetwork.Destroy(cardInfo.gameObject));
            return InstantiateCard(cardInfo.transform.position, cardInfo.transform.rotation, random.Next(int.MaxValue), cardInfo.transform.localScale);
        }
    }

    public class RandomStatsCard : MonoBehaviour, IPunInstantiateMagicCallback {
        public string StatGenName;

        public void OnPhotonInstantiate(PhotonMessageInfo info) {
            var data = info.photonView.InstantiationData;
            if(data == null) return;

            var seed = (int)data[0];
            var localScale = (Vector3)data[1];

            gameObject.transform.localScale = localScale;
            LoggerUtils.LogInfo($"Generating generatedRandom generatedCardInfo with seed {seed} using stat generator {StatGenName}");

            bool doesGeneratorExist = RandomCardsGenerator.RandomStatCardGenerators.TryGetValue(StatGenName, out var handler);
            if(doesGeneratorExist) {
                GenerateCard(handler, seed);
            } else {
                LoggerUtils.LogError($"Stat generator {StatGenName} does not exist.");
            }
        }

        private void GenerateCard(RandomCardsGenerator generator, int seed) {
            var cardInfo = GetComponent<CardInfo>();

            generator.GenerateRandomStats(seed, null, (generatedCardInfo) => {
                CardInfoStat[] stats = generatedCardInfo.CardInfo.cardStats;
                System.Random generatedRandom = new System.Random(seed);

                generator.ApplyRandomStats(cardInfo, generatedRandom);
                LoggerUtils.LogInfo($"Applied {stats.Length} stats to CardInfo {cardInfo.cardName}.");


                cardInfo.sourceCard = generatedCardInfo.CardInfo;
                cardInfo.cardStats = stats;
                generatedCardInfo.RandomCardsGenerator.OnCardGenerated?.Invoke(generatedCardInfo);


                CardInfoDisplayer displayer = gameObject.GetComponentInChildren<CardInfoDisplayer>();
                displayer.DrawCard(stats, generator.randomCardInfo.CardName, generator.randomCardInfo.CardDescription);
                LoggerUtils.LogInfo($"Card {cardInfo.cardName} displayed with {stats.Length} stats.");
            });
        }
    }

}
