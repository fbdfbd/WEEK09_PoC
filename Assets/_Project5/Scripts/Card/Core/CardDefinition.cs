using UnityEngine;

public abstract class CardDefinition : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string displayName;
    [SerializeField] private CardTag[] tags;
    [SerializeField] private CardComponentDefinition[] components;

    public string Id
    {
        get { return id; }
    }

    public string DisplayName
    {
        get { return displayName; }
    }

    public CardTag[] Tags
    {
        get { return tags; }
    }

    public CardComponentDefinition[] Components
    {
        get { return components; }
    }

    public virtual bool HasTag(CardTag tag)
    {
        if (tags == null)
        {
            return false;
        }

        for (int i = 0; i < tags.Length; i++)
        {
            if (tags[i] == tag)
            {
                return true;
            }
        }

        return false;
    }

    public T GetComponent<T>() where T : CardComponentDefinition
    {
        if (components == null)
        {
            return null;
        }

        for (int i = 0; i < components.Length; i++)
        {
            T component = components[i] as T;
            if (component != null)
            {
                return component;
            }
        }

        return null;
    }

    protected void SetupBase(string newId, string newDisplayName, CardTag[] newTags)
    {
        id = newId;
        displayName = newDisplayName;
        tags = newTags;
    }
}
