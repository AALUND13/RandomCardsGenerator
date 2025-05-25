using RandomCardsGenerators.Cards;
using RandomCardsGenerators.Extensions;
using RandomCardsGenerators.Utils;
using RarityLib.Utils;
using System;
using System.Collections.Generic;
using UnboundLib;
using UnityEngine;

namespace RandomCardsGenerators {
    public struct RandomStatInfo {
        public RandomStatGenerator StatGenerator;
        public float Value;

        public RandomStatInfo(RandomStatGenerator statGenerator, float value) {
            StatGenerator = statGenerator;
            Value = value;
        }
    }
    public struct RandomCardOption {
        public string CardName;
        public string ModInitials;
        public string CardDescription;
        public string TwoLetterCode;
        public CardInfo.Rarity CardRarity;
        public int Min;
        public int Max;

        public RandomCardOption(string cardName, CardInfo.Rarity cardRarity, string modInitials, string cardDescription, string twoLetterCode, int min, int max) {
            CardName = cardName;
            CardRarity = cardRarity;
            ModInitials = modInitials;
            CardDescription = cardDescription;
            TwoLetterCode = twoLetterCode;
            Min = min;
            Max = max;
        }

        public RandomCardOption(string cardName, string cardRarity, string modInitials, string cardDescription, string twoLetterCode, int min, int max) :
            this(cardName, RarityUtils.GetRarity(cardRarity), modInitials, cardDescription, twoLetterCode, min, max) { }
    }
    public struct GeneratedCardInfo {
        public RandomCardsGenerator RandomCardsGenerator;
        public CardInfo CardInfo;
        public System.Random Random;
        public RandomStatInfo[] RandomStatInfos;
        public int Seed;

        public GeneratedCardInfo(RandomCardsGenerator randomCardsGenerator, CardInfo cardInfo, RandomStatInfo[] randomStatInfos, System.Random random, int seed) {
            RandomCardsGenerator = randomCardsGenerator;
            CardInfo = cardInfo;
            RandomStatInfos = randomStatInfos;
            Random = random;
            Seed = seed;
        }
    }

    public class RandomCardsGenerator {
        internal static readonly Dictionary<string, RandomCardsGenerator> RandomStatCardGenerators = new Dictionary<string, RandomCardsGenerator>();

        public readonly List<RandomStatGenerator> StatGenerators;
        public readonly RandomCardOption randomCardInfo;
        public readonly string CardGenName;

        public Action<GeneratedCardInfo> OnCardGenerated;

        public RandomCardsGenerator(string statGenName, RandomCardOption randomCardInfo, List<RandomStatGenerator> statGenerators) {
            if(RandomStatCardGenerators.ContainsKey(statGenName))
                throw new Exception($"A RandomCardsGenerators with the name {statGenName} already exists!");
            RandomStatCardGenerators.Add(statGenName, this);

            CardGenName = statGenName;
            StatGenerators = statGenerators;
            this.randomCardInfo = randomCardInfo;

            NetworkingManager.RegisterEvent($"{statGenName}_SyncEvent", (data) => {
                var seed = (int)data[0];
                var playerID = (int)data[1];

                Player player = PlayerManager.instance.players.Find(p => p.playerID == playerID);
                GenerateRandomStats(seed, player);
            });
        }

        public CardInfoStat[] GetCardStatsFromSeed(int seed, int min, int max) {
            System.Random random = new System.Random(seed);

            var selectedStats = SelectRandomStats(random, min, max);

            List<CardInfoStat> cardStats = new List<CardInfoStat>();
            foreach(var item in selectedStats) {
                cardStats.Add(new CardInfoStat {
                    stat = item.StatGenerator.StatName,
                    amount = item.StatGenerator.GetStatString(item.Value),
                    positive = item.StatGenerator.IsPositive(item.Value)
                });
            }

            return cardStats.ToArray();
        }

        public RandomStatInfo[] ApplyRandomStats(CardInfo cardInfo, System.Random random) {
            int minClamped = Mathf.Max(0, randomCardInfo.Min);
            int maxClamped = Mathf.Clamp(randomCardInfo.Max, minClamped, StatGenerators.Count);

            var statCard = cardInfo.gameObject.AddComponent<BuildRandomStatCard>();
            statCard.CardName = randomCardInfo.CardName;
            statCard.ModInitials = randomCardInfo.ModInitials;

            var gun = cardInfo.GetComponent<Gun>();
            var statModifiers = cardInfo.GetComponent<CharacterStatModifiers>();
            var applyCardStats = cardInfo.GetComponent<ApplyCardStats>();
            var block = cardInfo.GetComponent<Block>();

            LoggerUtils.LogInfo($"Generating random stats for {cardInfo.cardName}...");

            var selectedStats = SelectRandomStats(random, minClamped, maxClamped);
            var cardStats = new List<CardInfoStat>();

            foreach(var item in selectedStats) {
                item.StatGenerator.Apply(item.Value, cardInfo, gun, applyCardStats, statModifiers, block);
                cardStats.Add(new CardInfoStat {
                    stat = item.StatGenerator.StatName,
                    amount = item.StatGenerator.GetStatString(item.Value),
                    positive = item.StatGenerator.IsPositive(item.Value)
                });
            }
            cardInfo.cardStats = cardStats.ToArray();

            LoggerUtils.LogInfo($"Generated {cardStats.Count} stats for {cardInfo.cardName}.");

            return selectedStats;
        }

        public void GenerateRandomStats(int seed, Player player = null, Action<GeneratedCardInfo> onCardGenerated = null) {
            System.Random random = new System.Random(seed);

            GameObject cardGameObject = GameObject.Instantiate(Main.blankCardPrefab);
            GameObject.Destroy(cardGameObject.transform.GetChild(0).gameObject);
            GameObject.DontDestroyOnLoad(cardGameObject);

            var statCard = cardGameObject.GetComponent<CardInfo>();
            var buildRandomStatCard = cardGameObject.AddComponent<BuildRandomStatCard>();

            GeneratedCardHolder.AddCardToGenerated(CardGenName, statCard);

            statCard.cardName = $"{CardGenName} Card ({GeneratedCardHolder.GetGeneratedCards(CardGenName).Count})";
            statCard.cardDestription = randomCardInfo.CardDescription;
            statCard.rarity = randomCardInfo.CardRarity;
            ModdingUtils.Utils.Cards.instance.AddHiddenCard(statCard);

            var selectedStats = ApplyRandomStats(statCard, random);

            buildRandomStatCard.BuildUnityCard((cardInfo) => {
                GeneratedCardInfo GeneratedCardData = new GeneratedCardInfo(this, cardInfo, selectedStats, random, seed);
                onCardGenerated?.Invoke(GeneratedCardData);
                OnCardGenerated?.Invoke(GeneratedCardData);

                if(player != null) {
                    Main.instance.ExecuteAfterSeconds(0.2f, () => {
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, cardInfo, false, randomCardInfo.TwoLetterCode, 2f, 2f, true);
                    });
                }

                LoggerUtils.LogInfo("Card built!");
            });
        }

        private RandomStatInfo[] SelectRandomStats(System.Random random, int min, int max) {
            int count = random.Next(min, max);

            var selectedGenerator = new List<RandomStatGenerator>(count);
            var selected = new RandomStatInfo[count];

            for(int i = 0; i < count; i++) {
                int index = random.Next(StatGenerators.Count);
                if(!selectedGenerator.Contains(StatGenerators[index])) {
                    selectedGenerator.Add(StatGenerators[index]);
                    float value = random.NextFloat(StatGenerators[index].MinValue, StatGenerators[index].MaxValue);
                    if(!StatGenerators[index].ShouldApply(value)) continue;
                    selected[i] = new RandomStatInfo(StatGenerators[index], value);
                }
            }

            return selected;
        }
    }
}
