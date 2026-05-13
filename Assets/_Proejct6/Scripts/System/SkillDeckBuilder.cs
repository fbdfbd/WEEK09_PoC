using System.Collections.Generic;

public class SkillDeckBuilder
{
    public void BuildSkillDeck(BattleState state, IReadOnlyList<SkillCardDefinition> skillDefinitions)
    {
        if (state == null || skillDefinitions == null)
        {
            return;
        }

        state.SkillDeck.Clear();

        for (int i = 0; i < skillDefinitions.Count; i++)
        {
            if (skillDefinitions[i] == null)
            {
                continue;
            }

            state.SkillDeck.Add(new CardInstance(skillDefinitions[i]));
        }

        state.SkillDeck.Shuffle();
    }
}
