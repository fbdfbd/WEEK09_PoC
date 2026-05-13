using UnityEngine;

[CreateAssetMenu(fileName = "UsedNumberCountCondition", menuName = "Synergy/Condition/Used Number Count")]
public class UsedNumberCountCondition : SynergyCondition
{
    [SerializeField] private int _minimumCount = 4;

    public override bool IsMet(BattleContext context)
    {
        return context.State.UsedNumbersThisTurn.Count >= _minimumCount;
    }
}
