using System.Collections.Generic;

public class NumberRequirementChecker
{
    public bool CanUseNumber(SkillCardDefinition skill, CardInstance numberCard)
    {
        if (skill == null)
        {
            return false;
        }

        NumCardDefinition number = BattleState.GetNumDefinition(numberCard);
        if (number == null)
        {
            return false;
        }

        return CanUseNumberValue(skill, number.Number);
    }

    public bool CanUseNumbers(SkillCardDefinition skill, List<CardInstance> numberCards)
    {
        if (skill == null || numberCards == null)
        {
            return false;
        }

        if (numberCards.Count < skill.RequiredCount)
        {
            return false;
        }

        for (int i = 0; i < numberCards.Count; i++)
        {
            NumCardDefinition number = BattleState.GetNumDefinition(numberCards[i]);
            if (number == null)
            {
                return false;
            }

            if (CanUseNumberValue(skill, number.Number) == false)
            {
                return false;
            }
        }

        return true;
    }

    private bool CanUseNumberValue(SkillCardDefinition skill, int number)
    {
        if (skill.NumberRequirementType == NumberRequirementType.Any)
        {
            return true;
        }

        if (skill.NumberRequirementType == NumberRequirementType.GreaterOrEqual)
        {
            return number >= skill.RequiredNumber;
        }

        if (skill.NumberRequirementType == NumberRequirementType.LessOrEqual)
        {
            return number <= skill.RequiredNumber;
        }

        if (skill.NumberRequirementType == NumberRequirementType.Equal)
        {
            return number == skill.RequiredNumber;
        }

        if (skill.NumberRequirementType == NumberRequirementType.Odd)
        {
            return number % 2 == 1;
        }

        if (skill.NumberRequirementType == NumberRequirementType.Even)
        {
            return number % 2 == 0;
        }

        return true;
    }
}
