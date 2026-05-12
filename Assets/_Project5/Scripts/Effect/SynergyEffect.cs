using UnityEngine;

public abstract class SynergyEffect : ScriptableObject
{
    public abstract void Apply(BattleContext context);
}
