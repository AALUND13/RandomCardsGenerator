namespace RandomCardsGenerators.Utils {
    public static class RandomStatsUtils {
        public static float ScaleStatByIntensity(float intensity, float baseValue = 1) {
            return (baseValue / 2f) / (1f - intensity);
        }

    }
}