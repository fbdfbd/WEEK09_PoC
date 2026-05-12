using UnityEngine;

[CreateAssetMenu(fileName = "NumCardDefinition", menuName = "Project5/Cards/Num Card")]
public class NumCardDefinition : CardDefinition
{
    [SerializeField] private int value = 1;

    public int Value
    {
        get { return value; }
    }

    public override bool HasTag(CardTag tag)
    {
        if (tag == CardTag.Number)
        {
            return true;
        }

        return base.HasTag(tag);
    }

    public void Setup(int newValue)
    {
        value = newValue;
        SetupBase("num_" + value, value.ToString(), new CardTag[] { CardTag.Number });
        ClampValue();
    }

    private void OnValidate()
    {
        ClampValue();
    }

    private void ClampValue()
    {
        if (value < 1)
        {
            value = 1;
        }

        if (value > 10)
        {
            value = 10;
        }
    }
}
