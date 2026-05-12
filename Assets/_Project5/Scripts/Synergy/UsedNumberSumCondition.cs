using UnityEngine;

[CreateAssetMenu(fileName = "UsedNumberSumCondition", menuName = "Project5/Synergy Conditions/Used Number Sum")]
public class UsedNumberSumCondition : SynergyCondition
{
    [SerializeField] private CompareType compareType;
    [SerializeField] private int sum;

    public override bool IsMet(BattleContext context)
    {
        int currentSum = 0;

        for (int i = 0; i < context.State.UsedNumbersThisTurn.Count; i++)
        {
            currentSum += context.State.UsedNumbersThisTurn[i];
        }

        return CompareUtility.Compare(currentSum, compareType, sum);
    }

    public void Setup(CompareType newCompareType, int newSum)
    {
        compareType = newCompareType;
        sum = newSum;
    }
}
