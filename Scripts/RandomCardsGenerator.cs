using RandomCardsGenerators.Cards;
using RandomCardsGenerators.Extensions;
using RandomCardsGenerators.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public override string ToString() {
            return $"RandomStatInfo: {StatGenerator.StatName} | Value: {Value}";
        }
    }
    public struct RandomCardOption {
        public string CardName;
        public string ModInitials;
        public string CardDescription;
        public string TwoLetterCode;

        public CardInfo.Rarity CardRarity;
        public CardThemeColor.CardThemeColorType ColorTheme;

        public int Min;
        public int Max;

        public RandomCardOption(
            string cardName,
            string modInitials,
            string cardDescription,
            string twoLetterCode,
            int min,
            int max,
            CardInfo.Rarity cardRarity,
            CardThemeColor.CardThemeColorType colorTheme = CardThemeColor.CardThemeColorType.TechWhite
        ) {
            CardName = cardName;
            ModInitials = modInitials;
            CardDescription = cardDescription;
            TwoLetterCode = twoLetterCode;
            CardRarity = cardRarity;
            ColorTheme = colorTheme;
            Min = min;
            Max = max;
        }

        public override string ToString() {
            return $"RandomCardOption: {CardName} | Rarity: {CardRarity} | ModInitials: {ModInitials} | Description: {CardDescription} | TwoLetterCode: {TwoLetterCode} | Min: {Min} | Max: {Max}";
        }
    }
    public struct GeneratedCardInfo {
        public RandomCardsGenerator RandomCardsGenerator;
        public CardInfo CardInfo;
        public RandomStatInfo[] RandomStatInfos;

        public System.Random Random;
        public int Seed;

        public bool HasValue =>
            RandomCardsGenerator != null && CardInfo != null && RandomStatInfos != null && RandomStatInfos.Length > 0;

        public GeneratedCardInfo(RandomCardsGenerator randomCardsGenerator, CardInfo cardInfo, RandomStatInfo[] randomStatInfos, System.Random random, int seed) {
            RandomCardsGenerator = randomCardsGenerator;
            CardInfo = cardInfo;
            RandomStatInfos = randomStatInfos;
            Random = random;
            Seed = seed;
        }

        public override string ToString() {
            return $"GeneratedCardInfo: {RandomCardsGenerator.CardGenName} | CardName: {CardInfo.cardName} | Seed: {Seed} | RandomStatInfos: {string.Join(", ", RandomStatInfos.Select(x => x.StatGenerator.StatName))}";
        }
    }

    public class RandomCardsGenerator {
        internal const string SYNC_EVENT_FORMAT = "{0}_SyncEvent";
        internal const string CARD_NAME_FORMAT = "{0} Card ({1})";

        internal static readonly Dictionary<string, RandomCardsGenerator> RandomStatCardGenerators = new Dictionary<string, RandomCardsGenerator>();

        // Act like a cache for generated cards to prevents generating the same card multiple times.
        public readonly Dictionary<int, GeneratedCardInfo> GeneratedCards = new Dictionary<int, GeneratedCardInfo>();

        public readonly List<RandomStatGenerator> StatGenerators;
        public readonly RandomCardOption RandomCardOption;
        public readonly string CardGenName;

        public Action<GeneratedCardInfo> OnCardGenerated;

        public RandomCardsGenerator(string cardGenName, RandomCardOption randomCardOption, List<RandomStatGenerator> statGenerators) {
            if(RandomStatCardGenerators.ContainsKey(cardGenName))
                throw new Exception($"A RandomCardsGenerators with the name {cardGenName} already exists!");
            else if(string.IsNullOrEmpty(cardGenName) || string.IsNullOrWhiteSpace(cardGenName))
                throw new Exception("Card generator name cannot be null or empty!");

            string sanitizedName = $"{randomCardOption.ModInitials}_{cardGenName.Sanitize()}";

            RandomStatCardGenerators.Add(sanitizedName, this);

            this.RandomCardOption = randomCardOption;
            this.CardGenName = sanitizedName;
            StatGenerators = statGenerators;

            NetworkingManager.RegisterEvent(string.Format(SYNC_EVENT_FORMAT, cardGenName), (data) => {
                try {
                    var seed = (int)data[0];
                    var playerID = (int)data[1];

                    Player player = PlayerManager.instance.players.Find(p => p.playerID == playerID);
                    GenerateRandomCard(seed, player);
                } catch(Exception e) {
                    LoggerUtils.LogError($"Error generating random stats for {cardGenName}: {e}");
                }
            });
        }

        public void CreateRandomCard(int seed, Player player = null) {
            NetworkingManager.RaiseEvent(string.Format(SYNC_EVENT_FORMAT, CardGenName), seed, player.playerID);
        }
        public void CreateRandomCard(Player player = null) {
            int seed = UnityEngine.Random.Range(0, int.MaxValue);
            NetworkingManager.RaiseEvent(string.Format(SYNC_EVENT_FORMAT, CardGenName), seed, player.playerID);
        }

        /// <summary>
        /// Generates a random card with the given seed and player.
        /// <para>NOTE: THIS WILL NOT BE CALLED ON ALL CLIENTS, ONLY ON THE CLIENT THAT CALLED IT.</para>
        /// <para>Use <see cref="CreateRandomCard(int, Player)"/> or <see cref="CreateRandomCard(Player)"/> to sync the card generation across all clients.</para>
        /// </summary>
        public void GenerateRandomCard(int seed, Player player = null, Action<GeneratedCardInfo> onCardGenerated = null) {
            if(GeneratedCards.ContainsKey(seed)) {
                LoggerUtils.LogInfo($"Card with seed {seed} already generated for {CardGenName}. Returning existing card.");

                onCardGenerated?.Invoke(GeneratedCards[seed]);
                if (player != null)
                    ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, GeneratedCards[seed].CardInfo, false, RandomCardOption.TwoLetterCode, 2f, 2f, true);

                return;
            }

            GameObject cardGameObject = GameObject.Instantiate(Main.blankCardPrefab);
            GameObject.Destroy(cardGameObject.transform.GetChild(0).gameObject);
            GameObject.DontDestroyOnLoad(cardGameObject);

            var statCard = cardGameObject.GetComponent<CardInfo>();
            var buildRandomStatCard = cardGameObject.AddComponent<BuiltRandomCard>();

            statCard.cardName = string.Format(CARD_NAME_FORMAT, CardGenName, GeneratedCardHolder.GetGeneratedCards(CardGenName).Count);
            statCard.cardDestription = RandomCardOption.CardDescription;
            statCard.rarity = RandomCardOption.CardRarity;
            statCard.colorTheme = RandomCardOption.ColorTheme;
            ModdingUtils.Utils.Cards.instance.AddHiddenCard(statCard);

            var random = new System.Random(seed);
            var selectedStats = ApplyRandomStats(statCard, random);

            GeneratedCardInfo GeneratedCardData = new GeneratedCardInfo(this, statCard, selectedStats, random, seed);
            GeneratedCardHolder.AddCardToGenerated(CardGenName, GeneratedCardData);
            GeneratedCards[seed] = GeneratedCardData;

            buildRandomStatCard.BuildUnityCard((cardInfo) => {
                if(player != null) {
                    Main.instance.ExecuteAfterSeconds(0.2f, () => {
                        ModdingUtils.Utils.Cards.instance.AddCardToPlayer(player, cardInfo, false, RandomCardOption.TwoLetterCode, 2f, 2f, true);
                    });
                }

                LoggerUtils.LogInfo("Card built!");
            });
            
            onCardGenerated?.Invoke(GeneratedCardData);
            OnCardGenerated?.Invoke(GeneratedCardData);
        }

        public RandomStatInfo[] ApplyRandomStats(CardInfo cardInfo, System.Random random) {
            var statCard = cardInfo.gameObject.GetOrAddComponent<BuiltRandomCard>();
            statCard.CardName = RandomCardOption.CardName;
            statCard.ModInitials = RandomCardOption.ModInitials;

            var gun = cardInfo.GetComponent<Gun>();
            var statModifiers = cardInfo.GetComponent<CharacterStatModifiers>();
            var applyCardStats = cardInfo.GetComponent<ApplyCardStats>();
            var block = cardInfo.GetComponent<Block>();

            LoggerUtils.LogInfo($"Generating random stats for {cardInfo.cardName}...");

            int minClamped = Mathf.Max(0, RandomCardOption.Min);
            int maxClamped = Mathf.Clamp(RandomCardOption.Max, minClamped, StatGenerators.Count);

            var selectedStats = SelectRandomStats(random, minClamped, maxClamped);
            cardInfo.cardStats = new CardInfoStat[selectedStats.Length];

            for(int i = 0; i < selectedStats.Length; i++) {
                var item = selectedStats[i];
                item.StatGenerator.Apply(item.Value, cardInfo, gun, applyCardStats, statModifiers, block);

                cardInfo.cardStats[i] = new CardInfoStat {
                    stat = item.StatGenerator.StatName,
                    amount = item.StatGenerator.GetStatString(item.Value),
                    positive = item.StatGenerator.IsPositive(item.Value)
                };
                LoggerUtils.LogInfo($"Applied stat {item.StatGenerator.StatName} with value {item.Value}.");
            }
            LoggerUtils.LogInfo($"Generated {selectedStats.Length} stats for {cardInfo.cardName}.");

            return selectedStats;
        }

        private RandomStatInfo[] SelectRandomStats(System.Random random, int min, int max) {
            LoggerUtils.LogInfo($"Selecting random stats for {CardGenName}...");

            int clampedMin = Mathf.Clamp(min, 0, StatGenerators.Count);
            int clampedMax = Mathf.Clamp(max, clampedMin, StatGenerators.Count);
            int statsAmount = random.Next(clampedMin, clampedMax + 1);

            var selectedGenerator = new List<RandomStatGenerator>(statsAmount);
            var selected = new RandomStatInfo[statsAmount];

            while(selectedGenerator.Count < statsAmount) {
                int index = random.Next(StatGenerators.Count);

                if(!selectedGenerator.Contains(StatGenerators[index])) {
                    float value = random.NextFloat(StatGenerators[index].MinValue, StatGenerators[index].MaxValue);
                    if(!StatGenerators[index].ShouldApply(value)) continue;

                    selectedGenerator.Add(StatGenerators[index]);
                    selected[selectedGenerator.Count - 1] = new RandomStatInfo(StatGenerators[index], value);

                    LoggerUtils.LogInfo($"Selected stat {selected[selectedGenerator.Count - 1].StatGenerator.StatName} with value {selected[selectedGenerator.Count - 1].Value}.");
                }
            }
            LoggerUtils.LogInfo($"Selected {selectedGenerator.Count} stats for {CardGenName}.");

            return selected;
        }

        public override string ToString() {
            return $"RandomCardsGenerator: {CardGenName} | CardName: {RandomCardOption.CardName} | Rarity: {RandomCardOption.CardRarity} | Min: {RandomCardOption.Min} | Max: {RandomCardOption.Max}";
        }
    }
}
