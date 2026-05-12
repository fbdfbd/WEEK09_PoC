using UnityEngine;

[CreateAssetMenu(fileName = "HasLowMiddleHighCondition", menuName = "Project5/Synergy Conditions/Has Low Middle High")]
public class HasLowMiddleHighCondition : SynergyCondition
{
    public override bool IsMet(BattleContext context)
    {
        bool hasLow = false;
        bool hasMiddle = false;
        bool hasHigh = false;

        for (int i = 0; i < context.State.UsedNumbersThisTurn.Count; i++)
        {
            int value = context.State.UsedNumbersThisTurn[i];

            if (value >= 1 && value <= 3)
            {
                hasLow = true;
            }

            if (value >= 4 && value <= 6)
            {
                hasMiddle = true;
            }

            if (value >= 7 && value <= 10)
            {
                hasHigh = true;
            }
        }

        return hasLow && hasMiddle && hasHigh;
    }
}
