using UnityEngine;

namespace RandomCardsGenerators.StatsGroup {
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
    public class AmmoStatGenerator : RandomStatGenerator {
        public override string StatName => "Ammo";

        public AmmoStatGenerator(float minValue, float maxValue) : base(minValue, maxValue) { }

        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            gun.ammo += Mathf.RoundToInt(value);
        public override string GetStatString(float value) => GetStringValue(value, false);

        public override bool ShouldApply(float value) => Mathf.RoundToInt(value) != 0;
        public override bool IsPositive(float value) => value > 0;
    }
    public class AdditionalBlocksStatGenerator : RandomStatGenerator {
        public override string StatName => "Additiona Block";

        public AdditionalBlocksStatGenerator(float minValue, float maxValue) : base(minValue, maxValue) { }

        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            block.additionalBlocks += Mathf.RoundToInt(value);
        public override string GetStatString(float value) => GetStringValue(value, false);

        public override bool ShouldApply(float value) => Mathf.RoundToInt(value) != 0;
        public override bool IsPositive(float value) => value > 0;
    }
    public class ExtraLiveStatGenerator : RandomStatGenerator {
        public override string StatName => "Extra Live";

        public ExtraLiveStatGenerator(float minValue, float maxValue) : base(minValue, maxValue) { }

        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            statModifiers.respawns += Mathf.RoundToInt(value);
        public override string GetStatString(float value) => GetStringValue(value, false);

        public override bool ShouldApply(float value) => Mathf.RoundToInt(value) != 0;
        public override bool IsPositive(float value) => value > 0;
    }
    public class JumpStatGenerator : RandomStatGenerator {
        public override string StatName => "Jump";

        public JumpStatGenerator(float minValue, float maxValue) : base(minValue, maxValue) { }

        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            statModifiers.numberOfJumps += Mathf.RoundToInt(value);
        public override string GetStatString(float value) => GetStringValue(value, false);

        public override bool ShouldApply(float value) => Mathf.RoundToInt(value) != 0;
        public override bool IsPositive(float value) => value > 0;
    }
}
