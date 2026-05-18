using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_IntroSequence", menuName = "PoC7/SO_IntroSequence")]
public sealed class SO_IntroSequence : ScriptableObject
{
    [SerializeField][TextArea] private string[] _lines;

    public IReadOnlyList<string> Lines => _lines;
}
