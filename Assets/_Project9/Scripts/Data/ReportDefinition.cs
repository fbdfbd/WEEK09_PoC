using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project9.Data
{
    [CreateAssetMenu(menuName = "Project9/Report Definition", fileName = "ReportDefinition")]
    public sealed class ReportDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string title;
        [TextArea(2, 6)]
        [SerializeField] private string summary;
        [SerializeField] private ParagraphDefinition[] paragraphs = Array.Empty<ParagraphDefinition>();

        public string Id => id;
        public string Title => title;
        public string Summary => summary;
        public IReadOnlyList<ParagraphDefinition> Paragraphs => paragraphs;
    }
}
