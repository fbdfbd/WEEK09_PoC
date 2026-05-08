using App.Gameplay.Runtime;

namespace App.Gameplay.Effects
{
    public sealed class EffectProcessor
    {
        public void Apply(EffectDefinition effect, GameRuntimeState runtimeState)
        {
            if (effect == null || runtimeState == null)
            {
                return;
            }

            switch (effect.Type)
            {
                case EffectType.ChangeStat:
                    runtimeState.Character.AddStat(effect.StatType, effect.Value);
                    break;

                case EffectType.SetStat:
                    runtimeState.Character.SetStat(effect.StatType, effect.Value);
                    break;

                case EffectType.AddFlag:
                    runtimeState.AddFlag(effect.TargetId);
                    break;

                case EffectType.RemoveFlag:
                    runtimeState.RemoveFlag(effect.TargetId);
                    break;

                case EffectType.AllowTarget:
                    runtimeState.Environment.Allow(effect.TargetId);
                    break;

                case EffectType.BlockTarget:
                    runtimeState.Environment.Block(effect.TargetId);
                    break;

                case EffectType.SetModifier:
                    runtimeState.Environment.SetModifier(effect.TargetId, effect.Value);
                    break;

                case EffectType.AddModifier:
                    runtimeState.Environment.AddModifier(effect.TargetId, effect.Value);
                    break;

                case EffectType.AddReport:
                    runtimeState.AddReport(new App.Gameplay.Reports.ReportEntry(
                        runtimeState.CurrentWeekIndex,
                        effect.Title,
                        effect.Body));
                    break;
            }
        }
    }
}
