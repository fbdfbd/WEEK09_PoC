using UnityEngine;

[CreateAssetMenu(fileName = "OrderNumberCondition", menuName = "Project5/Synergy Conditions/Order Number")]
public class OrderNumberCondition : SynergyCondition
{
    [SerializeField] private CompareType compareType;
    [SerializeField] private int value;

    public override bool IsMet(BattleContext context)
    {
        int orderValue = context.State.GetOrderNumberValue();
        return CompareUtility.Compare(orderValue, compareType, value);
    }
}
