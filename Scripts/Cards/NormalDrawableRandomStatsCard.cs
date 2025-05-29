using System.Collections.Generic;

namespace RandomCardsGenerators.Cards {
    public class NormalDrawableRandomStatsCard : DrawableRandomStatsCard {
        public static List<NormalDrawableRandomStatsCard> NormalDrawableCards = new List<NormalDrawableRandomStatsCard>();
        public NormalDrawableRandomStatsCard(RandomCardsGenerator statCardGenerator) : base(statCardGenerator) {
            NormalDrawableCards.Add(this);
        }
    }
}
