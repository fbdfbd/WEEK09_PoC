using UnityEngine;

public abstract class SynergyCondition : ScriptableObject
{
    public abstract bool IsMet(BattleContext context);
}
