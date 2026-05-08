using App.Gameplay.Environment;
using App.Gameplay.Runtime;

namespace App.Gameplay.Conditions
{
    public sealed class ConditionEvaluator
    {
        public bool IsMet(ConditionDefinition condition, GameRuntimeState runtimeState)
        {
            if (condition == null)
            {
                return true;
            }

            return HasRequiredFlags(condition, runtimeState) &&
                   HasNoBlockedFlags(condition, runtimeState) &&
                   MeetsStats(condition, runtimeState) &&
                   MeetsEnvironment(condition, runtimeState);
        }

        private static bool HasRequiredFlags(ConditionDefinition condition, GameRuntimeState runtimeState)
        {
            foreach (var flagId in condition.RequiredFlags ?? System.Array.Empty<string>())
            {
                if (!runtimeState.HasFlag(flagId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool HasNoBlockedFlags(ConditionDefinition condition, GameRuntimeState runtimeState)
        {
            foreach (var flagId in condition.BlockedFlags ?? System.Array.Empty<string>())
            {
                if (runtimeState.HasFlag(flagId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool MeetsStats(ConditionDefinition condition, GameRuntimeState runtimeState)
        {
            foreach (var requirement in condition.StatRequirements ?? System.Array.Empty<StatRequirement>())
            {
                if (requirement == null)
                {
                    continue;
                }

                var value = runtimeState.Character.GetStat(requirement.StatType);
                if (requirement.UseMinimum && value < requirement.Minimum)
                {
                    return false;
                }

                if (requirement.UseMaximum && value > requirement.Maximum)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool MeetsEnvironment(ConditionDefinition condition, GameRuntimeState runtimeState)
        {
            foreach (var requirement in condition.EnvironmentRequirements ?? System.Array.Empty<EnvironmentRequirement>())
            {
                if (requirement == null)
                {
                    continue;
                }

                if (requirement.Type == EnvironmentControlType.Allow &&
                    !Contains(runtimeState.Environment.AllowedTargets, requirement.TargetId))
                {
                    return false;
                }

                if (requirement.Type == EnvironmentControlType.Block &&
                    !Contains(runtimeState.Environment.BlockedTargets, requirement.TargetId))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool Contains(System.Collections.Generic.IReadOnlyList<string> values, string targetId)
        {
            if (string.IsNullOrWhiteSpace(targetId))
            {
                return false;
            }

            foreach (var value in values)
            {
                if (string.Equals(value, targetId, System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
