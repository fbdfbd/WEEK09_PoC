using System;
using System.Collections.Generic;

namespace App.Gameplay.Runtime
{
    public sealed class EnvironmentState
    {
        private readonly List<string> allowedTargets = new();
        private readonly List<string> blockedTargets = new();
        private readonly List<EnvironmentModifierState> modifiers = new();

        public IReadOnlyList<string> AllowedTargets => allowedTargets;
        public IReadOnlyList<string> BlockedTargets => blockedTargets;
        public IReadOnlyList<EnvironmentModifierState> Modifiers => modifiers;

        public void Allow(string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
            {
                return;
            }

            blockedTargets.Remove(targetId);
            AddUnique(allowedTargets, targetId);
        }

        public void Block(string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
            {
                return;
            }

            allowedTargets.Remove(targetId);
            AddUnique(blockedTargets, targetId);
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

            var modifier = modifiers.Find(item => item.TargetId == targetId);
            if (modifier != null)
            {
                return modifier;
            }

            modifier = new EnvironmentModifierState(targetId, 0);
            modifiers.Add(modifier);
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
