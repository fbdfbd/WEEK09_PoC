using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_PoCDatabase", menuName = "PoC7/SO_PoCDatabase")]
public class SO_PoCDatabase : ScriptableObject
{
    [SerializeField] private int _requestsPerDay = 3;
    [SerializeField] private List<SO_Request> _requests = new();
    [SerializeField] private List<SO_Agency> _agencies = new();   

    public int RequestsPerDay => _requestsPerDay;
    public IReadOnlyList<SO_Request> Requests => _requests;
    public IReadOnlyList<SO_Agency> Agencies => _agencies;
}
