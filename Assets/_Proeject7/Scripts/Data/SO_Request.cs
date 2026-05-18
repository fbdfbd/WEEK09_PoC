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
    [SerializeField] private string _deferredFollowUpRequestId;
    [SerializeField] private int _priority;
    [SerializeField] private string _relatedAgencyId;
    [SerializeField] private int _deadlineDays = 2;

    [SerializeField] private string _title;
    [SerializeField][TextArea] private string _body;
    [SerializeField][TextArea] private string _summary;

    [SerializeField] List<string> _tags = new();
    [SerializeField] private List<RequestFactTag> _factTags = new();

    public string Id => _id;
    public int Day => _day;
    public bool IsFollowUp => _isFollowUp;
    public string ParentRequestId => _parentRequestId;
    public string SupplementFollowUpRequestId => _supplementFollowUpRequestId;
    public string DeferredFollowUpRequestId => _deferredFollowUpRequestId;
    public int Priority => _priority;
    public string RelatedAgencyId => _relatedAgencyId;
    public int DeadlineDays => _deadlineDays;
    public string Title => _title;
    public string Body => _body;
    public string Summary => _summary;
    public List<string> Tags => _tags;
    public IReadOnlyList<RequestFactTag> FactTags => _factTags;
}
