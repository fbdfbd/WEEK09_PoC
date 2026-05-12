using UnityEngine;

[CreateAssetMenu(fileName = "HasStraightCondition", menuName = "Project5/Synergy Conditions/Has Straight")]
public class HasStraightCondition : SynergyCondition
{
    [SerializeField] private int straightLength = 3;

    public override bool IsMet(BattleContext context)
    {
        for (int startValue = 1; startValue <= 10; startValue++)
        {
            int foundCount = 0;

            for (int value = startValue; value < startValue + straightLength; value++)
            {
                if (HasNumber(context, value))
                {
                    foundCount++;
                }
            }

            if (foundCount == straightLength)
            {
                return true;
            }
        }

        return false;
    }

    public void Setup(int newStraightLength)
    {
        straightLength = newStraightLength;
    }

    private bool HasNumber(BattleContext context, int value)
    {
        for (int i = 0; i < context.State.UsedNumbersThisTurn.Count; i++)
        {
            if (context.State.UsedNumbersThisTurn[i] == value)
            {
                return true;
            }
        }

        return false;
    }
}
