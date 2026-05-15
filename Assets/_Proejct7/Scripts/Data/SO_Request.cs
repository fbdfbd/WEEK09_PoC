using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SO_Request", menuName = "PoC7/SO_Request")]
public class SO_Request : ScriptableObject
{
    [SerializeField] private string _id;

    [SerializeField] private string _title;
    [SerializeField][TextArea] private string _body;
    [SerializeField][TextArea] private string _summary;

    [SerializeField] List<string> _tags = new();

    public string Id => _id;
    public string Title => _title;
    public string Body => _body;
    public string Summary => _summary;
    public List<string> Tags => _tags;
}
