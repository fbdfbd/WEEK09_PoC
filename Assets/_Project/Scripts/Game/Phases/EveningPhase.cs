using System.Collections.Generic;
using App.Foundation.Data;
using App.Gameplay.Conditions;
using App.Gameplay.Definitions;
using App.Gameplay.Effects;
using App.Gameplay.Runtime;

namespace App.Gameplay.Phases
{
    public sealed class EveningPhase
    {
        private readonly DialoguePlayer _dialoguePlayer;

        public EveningPhase(DialoguePlayer dialoguePlayer)
        {
            _dialoguePlayer = dialoguePlayer;
        }

        public DialogueDefinition CurrentDialogue => _dialoguePlayer.CurrentDialogue;
        public DialogueNodeDefinition CurrentNode => _dialoguePlayer.CurrentNode;

        public DialogueNodeDefinition Enter(WeekDefinition week)
        {
            return _dialoguePlayer.Enter(week?.EveningDialogues);
        }

        public PhaseResult SelectChoice(int choiceIndex)
        {
            return _dialoguePlayer.SelectChoice(choiceIndex);
        }

        public bool IsCompleted => _dialoguePlayer.IsCompleted;
    }

    public sealed class IntroPhase
    {
        private readonly IDataRegistry _dataRegistry;
        private readonly DialoguePlayer _dialoguePlayer;

        public IntroPhase(IDataRegistry dataRegistry, DialoguePlayer dialoguePlayer)
        {
            _dataRegistry = dataRegistry;
            _dialoguePlayer = dialoguePlayer;
        }

        public DialogueDefinition CurrentDialogue => _dialoguePlayer.CurrentDialogue;
        public DialogueNodeDefinition CurrentNode => _dialoguePlayer.CurrentNode;

        public DialogueNodeDefinition Enter()
        {
            return _dialoguePlayer.Enter(_dataRegistry.GetIntroDialogue());
        }

        public PhaseResult SelectChoice(int choiceIndex)
        {
            return _dialoguePlayer.SelectChoice(choiceIndex);
        }

        public bool IsCompleted => _dialoguePlayer.IsCompleted;
    }

    public sealed class DialoguePlayer
    {
        private readonly ContentSelector _contentSelector;
        private readonly EffectProcessor _effectProcessor;
        private readonly GameRuntimeState _runtimeState;

        public DialoguePlayer(
            ContentSelector contentSelector,
            EffectProcessor effectProcessor,
            GameRuntimeState runtimeState)
        {
            _contentSelector = contentSelector;
            _effectProcessor = effectProcessor;
            _runtimeState = runtimeState;
        }

        public DialogueDefinition CurrentDialogue { get; private set; }
        public DialogueNodeDefinition CurrentNode { get; private set; }

        public DialogueNodeDefinition Enter(IReadOnlyList<DialogueDefinition> dialogues)
        {
            CurrentDialogue = _contentSelector.SelectHighestPriority(
                dialogues,
                dialogue => dialogue.Conditions,
                dialogue => dialogue.Priority);
            CurrentNode = CurrentDialogue?.FirstNode;
            return CurrentNode;
        }

        public DialogueNodeDefinition Enter(DialogueDefinition dialogue)
        {
            CurrentDialogue = dialogue;
            CurrentNode = CurrentDialogue?.FirstNode;
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
            _effectProcessor.ApplyAll(choice.Effects, _runtimeState);
            CurrentNode = choice.NextNode;
            return PhaseResult.Success();
        }

        public bool IsCompleted => CurrentNode == null ||
                                   CurrentNode.Choices == null ||
                                   CurrentNode.Choices.Length == 0;
    }
}
