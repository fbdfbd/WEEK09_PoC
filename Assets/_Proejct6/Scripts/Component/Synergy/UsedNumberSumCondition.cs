using UnityEngine;

[CreateAssetMenu(fileName = "UsedNumberSumCondition", menuName = "Synergy/Condition/Used Number Sum")]
public class UsedNumberSumCondition : SynergyCondition
{
    [SerializeField] private int _maximumSum = 15;

    public override bool IsMet(BattleContext context)
    {
        int sum = 0;

        for (int i = 0; i < context.State.UsedNumbersThisTurn.Count; i++)
        {
            sum += context.State.UsedNumbersThisTurn[i];
        }

        return sum <= _maximumSum;
    }
}
