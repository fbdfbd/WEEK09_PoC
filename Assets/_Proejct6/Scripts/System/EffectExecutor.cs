using System.Collections.Generic;

public class EffectExecutor
{
    public void Execute(
        IReadOnlyList<CardEffect> effects,
        BattleContext context,
        SkillResolveData data,
        EffectTiming timing)
    {
        if (effects == null)
        {
            return;
        }

        for (int i = 0; i < effects.Count; i++)
        {
            CardEffect effect = effects[i];
            if (effect == null)
            {
                continue;
            }

            if (effect.Timing == timing)
            {
                effect.Apply(context, data);
            }
        }
    }
}
