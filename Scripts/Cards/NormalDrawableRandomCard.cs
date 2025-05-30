using System.Collections.Generic;

namespace RandomCardsGenerators.Cards {
    public class NormalDrawableRandomCard : DrawableRandomCard {
        public static List<NormalDrawableRandomCard> NormalDrawableCards = new List<NormalDrawableRandomCard>();
        public NormalDrawableRandomCard(RandomCardsGenerator statCardGenerator, bool createToggleCard = true) : base(statCardGenerator) {
            NormalDrawableCards.Add(this);
        }
    }
}
