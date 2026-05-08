using System;
using UnityEngine;

namespace App.Gameplay.Definitions
{
    [CreateAssetMenu(fileName = "WeekDefinition_", menuName = "App/Data/Week Definition")]
    public sealed class WeekDefinition : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private int _weekIndex = 1;
        [SerializeField] private string _title;
        [SerializeField] private MorningIssueDefinition[] _morningIssues = Array.Empty<MorningIssueDefinition>();
        [SerializeField] private NoonActionDefinition[] _noonActions = Array.Empty<NoonActionDefinition>();
        [SerializeField] private DialogueDefinition[] _eveningDialogues = Array.Empty<DialogueDefinition>();

        public string Id => _id;
        public int WeekIndex => _weekIndex;
        public string Title => _title;
        public MorningIssueDefinition[] MorningIssues => _morningIssues;
        public NoonActionDefinition[] NoonActions => _noonActions;
        public DialogueDefinition[] EveningDialogues => _eveningDialogues;
    }
}
