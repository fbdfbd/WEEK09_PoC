using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Card/Skill Card Pool")]
public class SkillCardPool : ScriptableObject
{
    [SerializeField] private List<SkillCardDefinition> _cards;

    public List<SkillCardDefinition> Cards => _cards;

    public SkillCardDefinition GetById(string id)
    {
        foreach (SkillCardDefinition card in _cards)
        {
            if (card.Id == id)
            {
                return card;
            }
        }

        return null;
    }
}
