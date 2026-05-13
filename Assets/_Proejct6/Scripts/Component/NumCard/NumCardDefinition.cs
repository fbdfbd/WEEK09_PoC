using UnityEngine;

[CreateAssetMenu(fileName = "NumCardDefinition", menuName = "Scriptable Objects/NumCardDefinition")]
public class NumCardDefinition : CardDefinition
{
    [SerializeField] private int _number;

    public int Number => _number;
}
