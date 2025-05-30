using CardChoiceSpawnUniqueCardPatch.CustomCategories;
using RandomCardsGenerators.Utils;
using System.Collections.Generic;
using UnboundLib.Cards;
using UnboundLib.Utils;
using UnityEngine;

namespace RandomCardsGenerators.Cards {
    public class ToggleCard {
        public static readonly CardCategory ToggleCardCategory = CustomCardCategories.instance.CardCategory("ToggleCardCategory");
        internal static readonly List<ToggleCard> ToggleCards = new List<ToggleCard>();

        public readonly CardInfo toggleCardInfo;
        public List<CardInfo> cardsInToggle = new List<CardInfo>();

        public bool IsEnabled => toggleCardInfo != null && CardManager.IsCardActive(toggleCardInfo);

        public ToggleCard(List<CardInfo> cards, string cardName, string cardDescription, string modInitials, CardThemeColor.CardThemeColorType colorTheme = CardThemeColor.CardThemeColorType.TechWhite) {
            GameObject cardGameObject = GameObject.Instantiate(Main.blankCardPrefab);
            GameObject.Destroy(cardGameObject.transform.GetChild(0).gameObject);
            GameObject.DontDestroyOnLoad(cardGameObject);

            var cardInfo = cardGameObject.GetComponent<CardInfo>();
            var card = cardGameObject.AddComponent<ToggleCustomCard>();

            cardInfo.cardName = cardName;
            cardInfo.cardDestription = cardDescription;
            cardInfo.cardBase = Main.blankCardPrefab.GetComponent<CardInfo>().cardBase;
            cardInfo.categories = new CardCategory[] { ToggleCardCategory };
            cardInfo.rarity = CardInfo.Rarity.Common;
            cardInfo.colorTheme = colorTheme;
            card.ModInitials = modInitials;

            card.RegisterUnityCard((cardInfo) => {

                LoggerUtils.LogInfo($"Register toggle card {cardName} with description: {cardDescription} and mod initials: {modInitials}");
            });

            cardsInToggle = cards;
            ToggleCards.Add(this);

            toggleCardInfo = cardInfo;
        }

        public bool IsCardEnabledInstance(CardInfo cardInfo) {
            if(cardInfo == null) return false;

            CardInfo card = cardInfo;
            if(card.sourceCard != null) {
                card = card.sourceCard;
            }

            if(cardsInToggle.Contains(card)) {
                return IsEnabled;
            }

            return true;
        }

        public static bool IsCardEnabled(CardInfo cardInfo) {
            CardInfo card = cardInfo;
            if(card.sourceCard != null) {
                card = card.sourceCard;
            }
            foreach(var toggleCard in ToggleCards) {
                if(!toggleCard.IsCardEnabledInstance(card)) return false;
            }
            return true;
        }
    }

    internal class ToggleCustomCard : CustomCard {
        public string ModInitials;

        public override void OnAddCard(Player player, Gun gun, GunAmmo gunAmmo, CharacterData data, HealthHandler health, Gravity gravity, Block block, CharacterStatModifiers characterStats) { }

        protected override GameObject GetCardArt() {
            return cardInfo.cardArt;
        }

        protected override string GetDescription() {
            return cardInfo.cardDestription;
        }

        protected override CardInfo.Rarity GetRarity() {
            return cardInfo.rarity;
        }

        protected override CardInfoStat[] GetStats() {
            return cardInfo.cardStats;
        }

        protected override CardThemeColor.CardThemeColorType GetTheme() {
            return cardInfo.colorTheme;
        }

        protected override string GetTitle() {
            return cardInfo.cardName;
        }

        public override string GetModName() {
            return ModInitials;
        }
    }
}
