using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project9.Data
{
    [Serializable]
    public sealed class ParagraphDefinition
    {
        [SerializeField] private string id;
        [SerializeField] private string title;
        [TextArea(3, 10)]
        [SerializeField] private string originalText;
        [SerializeField, Range(1, 5)] private int informationValue = 1;
        [SerializeField, Range(0, 5)] private int sensitivity;
        [SerializeField] private ParagraphTag[] tags = Array.Empty<ParagraphTag>();
        [SerializeField] private ParagraphEditOption[] editOptions = Array.Empty<ParagraphEditOption>();

        public string Id => id;
        public string Title => title;
        public string OriginalText => originalText;
        public int InformationValue => informationValue;
        public int Sensitivity => sensitivity;
        public IReadOnlyList<ParagraphTag> Tags => tags;
        public IReadOnlyList<ParagraphEditOption> EditOptions => editOptions;
    }
}
