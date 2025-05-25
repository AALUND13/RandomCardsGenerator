using UnityEngine;

namespace RandomCardsGenerators {
    public abstract class RandomStatGenerator {
        public abstract string StatName { get; }

        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public RandomStatGenerator(float minValue, float maxValue) {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public abstract void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block);
        public virtual string GetStatString(float value) => GetStringValue(value * 100);

        public abstract bool IsPositive(float value);

        public virtual bool ShouldApply(float value) {
            return value != 0;
        }

        protected string GetStringValue(float value, bool isPercentage = true) {
            return $"{(value > 0 ? "+" : "")}{Mathf.Round(value)}{(isPercentage ? "%" : "")}";
        }
    }
}
