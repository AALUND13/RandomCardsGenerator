using System.Linq;
using TMPro;
using UnboundLib;
using UnboundLib.Cards;
using UnityEngine;

namespace RandomCardsGenerators.Cards {
    internal class BuildRandomStatCard : CustomCard {
        public string ModInitials;
        public string CardName;

        public override void SetupCard(CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) {
            this.ExecuteAfterFrames(1, () => {
                TextMeshProUGUI[] allChildren = gameObject.GetComponentsInChildren<TextMeshProUGUI>();
                if(allChildren.Length > 0) {
                    allChildren.Where(obj => obj.gameObject.name == "Text_Name").FirstOrDefault().GetComponent<TextMeshProUGUI>().text = CardName;
                }
            });
        }

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
