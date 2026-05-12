using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public abstract void Apply(BattleContext context, SkillResolveData data);
}
