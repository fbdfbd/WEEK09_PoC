using UnityEngine;

[CreateAssetMenu(fileName = "SkillCardDefinition", menuName = "Project5/Cards/Skill Card")]
public class SkillCardDefinition : CardDefinition
{
    [SerializeField] private SkillCardRole role;
    [SerializeField] private int requiredNumberCount = 1;
    [SerializeField] private CardEffect[] effects;

    public SkillCardRole Role
    {
        get { return role; }
    }

    public int RequiredNumberCount
    {
        get { return requiredNumberCount; }
    }

    public CardEffect[] Effects
    {
        get { return effects; }
    }

    public override bool HasTag(CardTag tag)
    {
        if (tag == CardTag.Skill)
        {
            return true;
        }

        return base.HasTag(tag);
    }

    public void Setup(string id, string displayName, SkillCardRole newRole, int newRequiredNumberCount, CardEffect[] newEffects)
    {
        role = newRole;
        requiredNumberCount = newRequiredNumberCount;
        effects = newEffects;
        SetupBase(id, displayName, new CardTag[] { CardTag.Skill });
        ClampRequiredNumberCount();
    }

    private void OnValidate()
    {
        ClampRequiredNumberCount();
    }

    private void ClampRequiredNumberCount()
    {
        if (requiredNumberCount < 1)
        {
            requiredNumberCount = 1;
        }
    }
}
