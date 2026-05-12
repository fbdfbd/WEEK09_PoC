using UnityEngine;

[CreateAssetMenu(fileName = "SkillCardPool", menuName = "Project5/Cards/Skill Card Pool")]
public class SkillCardPool : ScriptableObject
{
    [SerializeField] private SkillCardDefinition[] cards;

    public SkillCardDefinition[] Cards
    {
        get { return cards; }
    }

    public SkillCardDefinition GetCard(string id)
    {
        if (cards == null)
        {
            return null;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            if (cards[i] != null && cards[i].Id == id)
            {
                return cards[i];
            }
        }

        return null;
    }
}
