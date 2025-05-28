using UnityEngine;

namespace RandomCardsGenerators.StatsGroup {
    public class HealthStatGenerator : RandomStatGenerator {
        public override string StatName => "Health";
        public float ThresholdToZero;

        public HealthStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.05f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            statModifiers.health += value;
        public override bool IsPositive(float value) => value > 0;
    }
    public class SizeStatGenerator : RandomStatGenerator {
        public override string StatName => "Size";
        public float ThresholdToZero;

        public SizeStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.05f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            statModifiers.sizeMultiplier += value;
        public override bool IsPositive(float value) => value < 0;
    }

    public class DamageStatGenerator : RandomStatGenerator {
        public override string StatName => "Damage";
        public float ThresholdToZero;

        public DamageStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.05f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            gun.damage += value;
        public override bool IsPositive(float value) => value > 0;

    }
    public class ReloadTimeStatGenerator : RandomStatGenerator {
        public override string StatName => "Reload Time";
        public float ThresholdToZero;

        public ReloadTimeStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.05f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            gun.reloadTime += value;
        public override bool IsPositive(float value) => value < 0;
    }
    public class AttackSpeedStatGenerator : RandomStatGenerator {
        public override string StatName => "Attack Speed";
        public float ThresholdToZero;

        public AttackSpeedStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.05f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            gun.attackSpeed += value;
        public override bool IsPositive(float value) => value < 0;
    }
    public class MovementSpeedStatGenerator : RandomStatGenerator {
        public override string StatName => "Movement Speed";
        public float ThresholdToZero;

        public MovementSpeedStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.05f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            statModifiers.movementSpeed += value;
        public override bool IsPositive(float value) => value > 0;
    }
    public class BlockCooldownStatGenerator : RandomStatGenerator {
        public override string StatName => "Block Cooldown";
        public float ThresholdToZero;

        public BlockCooldownStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.025f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            block.cdMultiplier += value;
        public override bool IsPositive(float value) => value < 0;
    }
    public class BulletSpeedStatGenerator : RandomStatGenerator {
        public override string StatName => "Bullet Speed";
        public float ThresholdToZero;

        public BulletSpeedStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.05f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            gun.projectileSpeed += value;
        public override bool IsPositive(float value) => value > 0;
    }
    public class RegenStatGenerator : RandomStatGenerator {
        public override string StatName => "Regen";
        public float ThresholdToZero;

        public RegenStatGenerator(float minValue, float maxValue, float thresholdToZero = 1f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            statModifiers.regen += value;
        public override string GetStatString(float value) => GetStringValue(value, false);
        public override bool IsPositive(float value) => value > 0;
    }
    public class SpreadStatGenerator : RandomStatGenerator {
        public override string StatName => "Spread";
        public float ThresholdToZero;

        public SpreadStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.05f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            gun.spread += value;
        public override bool IsPositive(float value) => value < 0;
    }
    public class DragStatGenerator : RandomStatGenerator {
        public override string StatName => "Drag";
        public float ThresholdToZero;

        public DragStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.05f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            gun.drag += value;
        public override bool IsPositive(float value) => value < 0;
    }
    public class LifeSteelStatGenerator : RandomStatGenerator {
        public override string StatName => "Life Steal";
        public float ThresholdToZero;

        public LifeSteelStatGenerator(float minValue, float maxValue, float thresholdToZero = 0.05f) : base(minValue, maxValue) {
            ThresholdToZero = thresholdToZero;
        }

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= ThresholdToZero;
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            statModifiers.lifeSteal += value;
        public override bool IsPositive(float value) => value > 0;
    }
}
