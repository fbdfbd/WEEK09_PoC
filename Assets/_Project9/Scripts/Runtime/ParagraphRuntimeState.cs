using System;
using Project9.Data;
using UnityEngine;

namespace Project9.Runtime
{
    public sealed class ParagraphRuntimeState
    {
        private const int BaseIntegrity = 100;

        public ParagraphRuntimeState(ParagraphDefinition definition)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }

        public ParagraphDefinition Definition { get; }
        public ParagraphEditOption SelectedEditOption { get; private set; }
        public ParagraphActionType CurrentActionType => SelectedEditOption?.ActionType ?? ParagraphActionType.Keep;
        public string CurrentText => GetCurrentText();
        public int CurrentIntegrity => Mathf.Clamp(BaseIntegrity + (SelectedEditOption?.IntegrityDelta ?? 0), 0, 100);
        public int CurrentExposure => Mathf.Max(0, Definition.Sensitivity + (SelectedEditOption?.ExposureDelta ?? 0));
        public int CurrentDistortionPenalty => SelectedEditOption?.DistortionPenalty ?? 0;
        public bool HasExplicitEdit => SelectedEditOption != null;

        public bool SelectEditOption(string editOptionId)
        {
            if (string.IsNullOrWhiteSpace(editOptionId))
            {
                ClearEditOption();
                return true;
            }

            foreach (var option in Definition.EditOptions)
            {
                if (option == null)
                {
                    continue;
                }

                if (string.Equals(option.Id, editOptionId, StringComparison.Ordinal))
                {
                    SelectedEditOption = option;
                    return true;
                }
            }

            return false;
        }

        public void SelectEditOption(ParagraphEditOption editOption)
        {
            SelectedEditOption = editOption;
        }

        public void ClearEditOption()
        {
            SelectedEditOption = null;
        }

        private string GetCurrentText()
        {
            if (SelectedEditOption == null || string.IsNullOrWhiteSpace(SelectedEditOption.ResultingText))
            {
                return Definition.OriginalText;
            }

            return SelectedEditOption.ResultingText;
        }
    }
}
