using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SO_Agency", menuName = "PoC7/SO_Agency")]
public class SO_Agency : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private string _name;
    [SerializeField] private int defaultRelation = 5;
    [SerializeField][TextArea] private string _tooltipText;
    [SerializeField] private List<string> _tags = new();

    public string Id => _id;
    public string Name => _name;
    public int DefaultRelation => defaultRelation;
    public string TooltipText => _tooltipText;
    public List<string> Tags => _tags;
}
