using System;
using System.Collections.Generic;

namespace App.Gameplay.Runtime
{
    public sealed class EnvironmentState
    {
        private readonly List<string> _allowedTargets = new();
        private readonly List<string> _blockedTargets = new();
        private readonly List<EnvironmentModifierState> _modifiers = new();

        public IReadOnlyList<string> AllowedTargets => _allowedTargets;
        public IReadOnlyList<string> BlockedTargets => _blockedTargets;
        public IReadOnlyList<EnvironmentModifierState> Modifiers => _modifiers;

        public void Allow(string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
            {
                return;
            }

            _blockedTargets.Remove(targetId);
            AddUnique(_allowedTargets, targetId);
        }

        public void Block(string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
            {
                return;
            }

            _allowedTargets.Remove(targetId);
            AddUnique(_blockedTargets, targetId);
        }

        public void SetModifier(string targetId, int value)
        {
            var modifier = GetOrCreateModifier(targetId);
            if (modifier == null)
            {
                return;
            }

            modifier.Value = value;
        }

        public void AddModifier(string targetId, int value)
        {
            var modifier = GetOrCreateModifier(targetId);
            if (modifier == null)
            {
                return;
            }

            modifier.Value += value;
        }

        private EnvironmentModifierState GetOrCreateModifier(string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
            {
                return null;
            }

            var modifier = _modifiers.Find(item => item.TargetId == targetId);
            if (modifier != null)
            {
                return modifier;
            }

            modifier = new EnvironmentModifierState(targetId, 0);
            _modifiers.Add(modifier);
            return modifier;
        }

        private static void AddUnique(List<string> values, string value)
        {
            foreach (var item in values)
            {
                if (string.Equals(item, value, StringComparison.Ordinal))
                {
                    return;
                }
            }

            values.Add(value);
        }
    }
}
