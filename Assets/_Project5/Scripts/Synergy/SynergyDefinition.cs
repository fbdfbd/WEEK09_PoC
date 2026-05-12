using UnityEngine;

[CreateAssetMenu(fileName = "SynergyDefinition", menuName = "Project5/Synergy/Synergy")]
public class SynergyDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private SynergyCondition[] conditions;
    [SerializeField] private SynergyEffect[] effects;

    public string Id
    {
        get { return id; }
    }

    public string DisplayName
    {
        get { return displayName; }
    }

    public bool CanActivate(BattleContext context)
    {
        if (conditions == null || conditions.Length == 0)
        {
            return true;
        }

        for (int i = 0; i < conditions.Length; i++)
        {
            if (conditions[i] != null && conditions[i].IsMet(context) == false)
            {
                return false;
            }
        }

        return true;
    }

    public void Apply(BattleContext context)
    {
        if (effects == null)
        {
            return;
        }

        for (int i = 0; i < effects.Length; i++)
        {
            if (effects[i] != null)
            {
                effects[i].Apply(context);
            }
        }
    }

    public void Setup(string newId, string newDisplayName, SynergyCondition[] newConditions, SynergyEffect[] newEffects)
    {
        id = newId;
        displayName = newDisplayName;
        conditions = newConditions;
        effects = newEffects;
    }
}
