using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SO_Agency", menuName = "PoC7/SO_Agency")]
public class SO_Agency : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private string _name;
    [SerializeField] private int defaultRelation;
    [SerializeField] private List<string> _tags = new();

    public string Id => _id;
    public string Name => _name;
    public int DefaultRelation => defaultRelation;
    public List<string> Tags => _tags;
}
