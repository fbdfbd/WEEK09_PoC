using UnityEngine;

[CreateAssetMenu(fileName = "UsedNumberCountCondition", menuName = "Project5/Synergy Conditions/Used Number Count")]
public class UsedNumberCountCondition : SynergyCondition
{
    [SerializeField] private CompareType compareType;
    [SerializeField] private int count;

    public override bool IsMet(BattleContext context)
    {
        int usedCount = context.State.UsedNumbersThisTurn.Count;
        return CompareUtility.Compare(usedCount, compareType, count);
    }

    public void Setup(CompareType newCompareType, int newCount)
    {
        compareType = newCompareType;
        count = newCount;
    }
}
