using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project9.Data
{
    [CreateAssetMenu(menuName = "Project9/Scenario Definition", fileName = "Project9ScenarioDefinition")]
    public sealed class Project9ScenarioDefinition : ScriptableObject
    {
        [SerializeField] private ReportDefinition report;
        [SerializeField] private SubmitTargetDefinition[] submitTargets = Array.Empty<SubmitTargetDefinition>();

        public ReportDefinition Report => report;
        public IReadOnlyList<SubmitTargetDefinition> SubmitTargets => submitTargets;
    }
}
