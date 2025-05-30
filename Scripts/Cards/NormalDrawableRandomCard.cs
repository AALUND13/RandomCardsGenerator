using System.Collections.Generic;

namespace RandomCardsGenerators.Cards {
    public class NormalDrawableRandomCard : DrawableRandomCard {
        public static List<NormalDrawableRandomCard> NormalDrawableCards = new List<NormalDrawableRandomCard>();

        /// <summary>
        /// This is the toggle card that will be created for this normal drawable card.
        /// Note: This can be null if the card was created without a toggle card.
        /// </summary>
        public ToggleCard ToggleCard;

        public NormalDrawableRandomCard(RandomCardsGenerator statCardGenerator, bool createToggleCard = true) : base(statCardGenerator) {
            if(createToggleCard) {
                var cardOption = statCardGenerator.RandomCardOption;
                ToggleCard = new ToggleCard(new List<CardInfo> { CardInfo }, cardOption.CardName, cardOption.CardDescription, cardOption.ModInitials, cardOption.ColorTheme);
                ToggleCard.toggleCardInfo.rarity = cardOption.CardRarity;
            }
            NormalDrawableCards.Add(this);
        }
    }
}
