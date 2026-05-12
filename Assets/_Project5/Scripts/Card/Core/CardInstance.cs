using System;

public class CardInstance
{
    private readonly string instanceId;
    private readonly CardDefinition definition;

    public string InstanceId
    {
        get { return instanceId; }
    }

    public CardDefinition Definition
    {
        get { return definition; }
    }

    public CardInstance(CardDefinition definition)
    {
        this.definition = definition;
        instanceId = Guid.NewGuid().ToString();
    }

    public bool HasTag(CardTag tag)
    {
        if (definition == null)
        {
            return false;
        }

        return definition.HasTag(tag);
    }
}
