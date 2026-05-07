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
            }
        }
    }
}
