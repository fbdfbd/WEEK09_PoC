using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SO_Request", menuName = "PoC7/SO_Request")]
public class SO_Request : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private int _day = 1;
    [SerializeField] private bool _isFollowUp;
    [SerializeField] private string _parentRequestId;
    [SerializeField] private string _supplementFollowUpRequestId;
    [SerializeField] private int _priority;

    [SerializeField] private string _title;
    [SerializeField][TextArea] private string _body;
    [SerializeField][TextArea] private string _summary;

    [SerializeField] List<string> _tags = new();

    public string Id => _id;
    public int Day => _day;
    public bool IsFollowUp => _isFollowUp;
    public string ParentRequestId => _parentRequestId;
    public string SupplementFollowUpRequestId => _supplementFollowUpRequestId;
    public int Priority => _priority;
    public string Title => _title;
    public string Body => _body;
    public string Summary => _summary;
    public List<string> Tags => _tags;
}
