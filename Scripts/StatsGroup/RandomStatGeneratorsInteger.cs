using UnityEngine;

namespace RandomCardsGenerators.StatsGroup {
    public class ReloadTimeSecondStatGenerator : RandomStatGenerator {
        public override string StatName => "Reload Time";
        public ReloadTimeSecondStatGenerator(float minValue, float maxValue) : base(minValue, maxValue) { }

        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            gun.reloadTimeAdd += value;
        public override string GetStatString(float value) => $"{(value > 0 ? "+" : "")}{value:F2}s";

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= 0.05f;
        public override bool IsPositive(float value) => value < 0;
    }
    public class BlockCooldownSecondStatGenerator : RandomStatGenerator {
        public override string StatName => "Block Cooldown";
        public BlockCooldownSecondStatGenerator(float minValue, float maxValue) : base(minValue, maxValue) { }
        
        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            block.cdAdd += value;
        public override string GetStatString(float value) => $"{(value > 0 ? "+" : "")}{value:F2}s";

        public override bool ShouldApply(float value) => Mathf.Abs(value) >= 0.05f;
        public override bool IsPositive(float value) => value < 0;
    }
    public class NumberOfProjectilesStatGenerator : RandomStatGenerator {
        public override string StatName => "Number of Projectiles";
        public NumberOfProjectilesStatGenerator(float minValue, float maxValue) : base(minValue, maxValue) { }

        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            gun.numberOfProjectiles += Mathf.RoundToInt(value);
        public override string GetStatString(float value) => GetStringValue(Mathf.RoundToInt(value), false);

        public override bool ShouldApply(float value) => Mathf.RoundToInt(value) != 0;
        public override bool IsPositive(float value) => value > 0;
    }
    public class BouncesStatGenerator : RandomStatGenerator {
        public override string StatName => "Bounces";
        public BouncesStatGenerator(float minValue, float maxValue) : base(minValue, maxValue) { }

        public override void Apply(float value, CardInfo cardInfo, Gun gun, ApplyCardStats cardStats, CharacterStatModifiers statModifiers, Block block) =>
            gun.reflects += Mathf.RoundToInt(value);
        public override string GetStatString(float value) => GetStringValue(Mathf.RoundToInt(value), false);

        public override bool ShouldApply(float value) => Mathf.RoundToInt(value) != 0;
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
