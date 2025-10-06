using Photon.Pun;
using RandomCardsGenerators.Utils;
using System.Collections.Generic;
using System.Linq;
using UnboundLib;
using UnityEngine;

namespace RandomCardsGenerators.Cards {
    /// <summary>
    /// By default, just by creating a new instance of this class will <strong>NOT</strong> make the random card appear in hands.
    /// <para>You have to implement that functionality yourself. Take a look at <see href="https://github.com/AALUND13/CorruptedCardsManager">CorruptedCardsManager</see> for an example.</para>
    /// <para>To be specific, Take a look at <see href="https://github.com/AALUND13/CorruptedCardsManager/blob/main/Scripts/Patches/SpawnUniqueCard.cs">SpawnUniqueCardPatch</see>
    /// and <see href="https://github.com/AALUND13/CorruptedCardsManager/blob/main/Scripts/CorruptedCardsGenerators.cs#L26">CorruptedCardsGenerators</see> classes.</para>
    /// </summary>
    public class DrawableRandomCard {
        internal static List<DrawableRandomCard> DrawableCards = new List<DrawableRandomCard>();
        internal static readonly System.Random random = new System.Random();

        public readonly RandomCardsGenerator StatCardGenerator;
        public readonly GameObject CardGameObject;
        public readonly CardInfo CardInfo;

        public DrawableRandomCard(RandomCardsGenerator statCardGenerator) {
            GameObject cardGameObject = GameObject.Instantiate(Main.blankCardPrefab);
            cardGameObject.name = $"__{statCardGenerator.RandomCardOption.ModInitials}__{statCardGenerator.CardGenName}_DrawableCard";

            GameObject.Destroy(cardGameObject.transform.GetChild(0).gameObject);
            GameObject.DontDestroyOnLoad(cardGameObject);

            var card = cardGameObject.AddComponent<RandomCard>();
            CardInfo = cardGameObject.GetComponent<CardInfo>();
            CardInfo.cardBase = Main.blankCardPrefab.GetComponent<CardInfo>().cardBase;
            CardInfo.rarity = statCardGenerator.RandomCardOption.CardRarity;
            CardInfo.cardName = statCardGenerator.RandomCardOption.CardName;
            CardInfo.cardDestription = statCardGenerator.RandomCardOption.CardDescription;
            CardInfo.colorTheme = statCardGenerator.RandomCardOption.ColorTheme;

            card.StatGenName = statCardGenerator.CardGenName;

            PhotonNetwork.PrefabPool.RegisterPrefab(cardGameObject.name, cardGameObject);

            CardGameObject = cardGameObject;
            StatCardGenerator = statCardGenerator;
            DrawableCards.Add(this);
        }

        public GameObject InstantiateCard(Vector3 position, Quaternion rotation, int seed, Vector3 localScale, Player requestPlayer = null) {
            return PhotonNetwork.Instantiate(
                CardGameObject.name,
                position,
                rotation,
                0,
                new object[] { seed, localScale, requestPlayer?.playerID }
            );
        }

        public GameObject ReplaceCard(CardInfo cardInfo, Player requestPlayer) {
            Main.instance.ExecuteAfterFrames(3, () => PhotonNetwork.Destroy(cardInfo.gameObject));
            return InstantiateCard(cardInfo.transform.position, cardInfo.transform.rotation, random.Next(int.MaxValue), cardInfo.transform.localScale, requestPlayer);
        }

        public GameObject ReplaceCard(CardInfo cardInfo) => ReplaceCard(cardInfo, null);
    }

    public class RandomCard : MonoBehaviour, IPunInstantiateMagicCallback {
        public string StatGenName;
        public bool IsInstantiate;

        public void OnPhotonInstantiate(PhotonMessageInfo info) {
            var data = info.photonView.InstantiationData;
            if(data == null) return;

            var seed = (int)data[0];
            var localScale = (Vector3)data[1];
            Player player = null;
            if(data[2] != null) {
                player = PlayerManager.instance.players.FirstOrDefault(player => player.playerID == (int)data[2]);
            }

            gameObject.transform.localScale = localScale;

            LoggerUtils.LogInfo($"Generating generatedRandom generatedCardInfo with seed {seed} using stat generator {StatGenName}");

            bool doesGeneratorExist = RandomCardsGenerator.RandomStatCardGenerators.TryGetValue(StatGenName, out var handler);
            if(doesGeneratorExist) {
                GenerateCard(handler, seed, player);
            } else {
                LoggerUtils.LogError($"Stat generator {StatGenName} does not exist.");
            }

            IsInstantiate = true;
        }

        private void GenerateCard(RandomCardsGenerator generator, int seed, Player requestPlayer) {
            var cardInfo = GetComponent<CardInfo>();

            generator.GenerateRandomCard(seed, requestPlayer, (generatedCardInfo) => {
                LoggerUtils.LogInfo($"CardGenerator: {generator}");
                LoggerUtils.LogInfo($"GeneratedCardInfo: {generatedCardInfo}");

                var newGeneratedCardInfo = new GeneratedCardInfo(generator, cardInfo, generatedCardInfo.RandomStatInfos, generatedCardInfo.Random, seed);
                var stats = generatedCardInfo.CardInfo.cardStats;
                var generatedRandom = new System.Random(seed);

                generator.ApplyRandomStats(cardInfo, generatedRandom);
                LoggerUtils.LogInfo($"Applied {stats.Length} stats to toggleCardInfo {cardInfo.cardName}.");

                cardInfo.sourceCard = generatedCardInfo.CardInfo;
                cardInfo.cardStats = stats;
                generatedCardInfo.RandomCardsGenerator.OnCardGenerated?.Invoke(newGeneratedCardInfo);

                CardInfoDisplayer displayer = gameObject.GetComponentInChildren<CardInfoDisplayer>();
                displayer.DrawCard(stats, generator.RandomCardOption.CardName, generator.RandomCardOption.CardDescription);
                LoggerUtils.LogInfo($"Card {cardInfo.cardName} displayed with {stats.Length} stats.");

                // This type only exist when the 'DeckCustomization' mod is installed, So we check if it exists before adding it.
                if(Main.RarityTextType != null) {
                    gameObject.AddComponent(Main.RarityTextType);
                }
            });
        }
    }

}
