using System;

public class CardInstance
{
    public CardDefinition Definition { get; private set; }
    public string InstanceId { get; private set; }

    public CardInstance(CardDefinition definition)
    {
        Definition = definition;
        InstanceId = Guid.NewGuid().ToString();
    }
}
