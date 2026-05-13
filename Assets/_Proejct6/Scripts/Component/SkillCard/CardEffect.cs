using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    [SerializeField] private EffectCategory _category;
    [SerializeField] private EffectTiming _timing;

    public EffectCategory Category => _category;
    public EffectTiming Timing => _timing;

    public abstract void Apply(BattleContext context, SkillResolveData data);
}
