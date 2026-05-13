using UnityEngine;

[CreateAssetMenu(fileName = "ConsecutiveNumbersCondition", menuName = "Synergy/Condition/Consecutive Numbers")]
public class ConsecutiveNumbersCondition : SynergyCondition
{
    [SerializeField] private int _requiredLength = 3;

    public override bool IsMet(BattleContext context)
    {
        for (int startNumber = 1; startNumber <= 10; startNumber++)
        {
            int foundCount = 0;

            for (int number = startNumber; number < startNumber + _requiredLength; number++)
            {
                if (HasNumber(context, number))
                {
                    foundCount++;
                }
            }

            if (foundCount == _requiredLength)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasNumber(BattleContext context, int number)
    {
        for (int i = 0; i < context.State.UsedNumbersThisTurn.Count; i++)
        {
            if (context.State.UsedNumbersThisTurn[i] == number)
            {
                return true;
            }
        }

        return false;
    }
}
