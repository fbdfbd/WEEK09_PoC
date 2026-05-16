using System;
using UnityEngine;

namespace Project9.Data
{
    [Serializable]
    public sealed class ParagraphEditOption
    {
        [SerializeField] private string id;
        [SerializeField] private string label;
        [SerializeField] private ParagraphActionType actionType;
        [TextArea(2, 6)]
        [SerializeField] private string resultingText;
        [SerializeField, Range(-100, 0)] private int integrityDelta;
        [SerializeField, Range(-5, 5)] private int exposureDelta;
        [SerializeField, Range(0, 100)] private int distortionPenalty;

        public string Id => id;
        public string Label => label;
        public ParagraphActionType ActionType => actionType;
        public string ResultingText => resultingText;
        public int IntegrityDelta => integrityDelta;
        public int ExposureDelta => exposureDelta;
        public int DistortionPenalty => distortionPenalty;
    }
}
