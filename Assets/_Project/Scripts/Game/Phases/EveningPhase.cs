using System.Linq;
using App.Gameplay.Conditions;
using App.Gameplay.Definitions;
using App.Gameplay.Effects;
using App.Gameplay.Runtime;

namespace App.Gameplay.Phases
{
    public sealed class EveningPhase
    {
        private readonly ConditionEvaluator _conditionEvaluator;
        private readonly EffectProcessor _effectProcessor;
        private readonly GameRuntimeState _runtimeState;

        public EveningPhase(
            ConditionEvaluator conditionEvaluator,
            EffectProcessor effectProcessor,
            GameRuntimeState runtimeState)
        {
            _conditionEvaluator = conditionEvaluator;
            _effectProcessor = effectProcessor;
            _runtimeState = runtimeState;
        }

        public DialogueDefinition CurrentDialogue { get; private set; }
        public DialogueNodeDefinition CurrentNode { get; private set; }

        public DialogueNodeDefinition Enter(WeekDefinition week)
        {
            CurrentDialogue = week?.EveningDialogues?
                .Where(dialogue => dialogue != null && _conditionEvaluator.IsMet(dialogue.Conditions, _runtimeState))
                .OrderByDescending(dialogue => dialogue.Priority)
                .FirstOrDefault();

            CurrentNode = CurrentDialogue?.GetNode(CurrentDialogue.FirstNodeId);
            return CurrentNode;
        }

        public PhaseResult SelectChoice(int choiceIndex)
        {
            if (CurrentNode == null)
            {
                return PhaseResult.Failed("Dialogue node is missing.");
            }

            if (CurrentNode.Choices == null || choiceIndex < 0 || choiceIndex >= CurrentNode.Choices.Length)
            {
                return PhaseResult.Failed("Dialogue choice index is invalid.");
            }

            var choice = CurrentNode.Choices[choiceIndex];
            foreach (var effect in choice.Effects ?? System.Array.Empty<EffectDefinition>())
            {
                _effectProcessor.Apply(effect, _runtimeState);
            }

            CurrentNode = CurrentDialogue?.GetNode(choice.NextNodeId);
            return PhaseResult.Success();
        }

        public bool IsCompleted => CurrentNode == null ||
                                   CurrentNode.Choices == null ||
                                   CurrentNode.Choices.Length == 0;
    }
}
