using System.Collections.Generic;
using UnityEngine;

namespace Project3.OscilloPatch
{
    public sealed class Mission
    {
        public string Title { get; }
        public string Description { get; }
        public IReadOnlyList<Vector2> TargetNodes { get; }
        public IReadOnlyList<MissionCondition> Conditions { get; }

        public Mission(string title, string description, IReadOnlyList<Vector2> targetNodes)
        {
            Title = title;
            Description = description;
            TargetNodes = targetNodes;
            Conditions = new MissionCondition[]
            {
                new FrequencyRatioCondition(2, 3),
                new TargetNodeCondition(targetNodes, 22f),
                new MaxAmplitudeCondition(150f),
                new ComplexityCondition(7),
            };
        }

        private sealed class TargetNodeCondition : MissionCondition
        {
            private readonly IReadOnlyList<Vector2> targetNodes;
            private readonly float hitRadius;

            public string Description => "\ud45c\uc2dc \uc811\uc810 2\uac1c \ud1b5\uacfc";

            public TargetNodeCondition(IReadOnlyList<Vector2> targetNodes, float hitRadius)
            {
                this.targetNodes = targetNodes;
                this.hitRadius = hitRadius;
            }

            public bool IsMet(SignalPair signals, IReadOnlyList<Vector2> points)
            {
                if (!signals.IsValid)
                {
                    return false;
                }

                foreach (Vector2 targetNode in targetNodes)
                {
                    if (!HasHit(targetNode, points))
                    {
                        return false;
                    }
                }

                return true;
            }

            private bool HasHit(Vector2 targetNode, IReadOnlyList<Vector2> points)
            {
                for (int index = 0; index < points.Count; index++)
                {
                    if (Vector2.Distance(targetNode, points[index]) <= hitRadius)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private sealed class MaxAmplitudeCondition : MissionCondition
        {
            private readonly float maxAmplitude;

            public string Description => $"과전압 영역 접촉 금지";

            public MaxAmplitudeCondition(float maxAmplitude)
            {
                this.maxAmplitude = maxAmplitude;
            }

            public bool IsMet(SignalPair signals, IReadOnlyList<Vector2> points)
            {
                if (!signals.IsValid)
                {
                    return false;
                }

                for (int index = 0; index < points.Count; index++)
                {
                    if (Mathf.Abs(points[index].x) > maxAmplitude || Mathf.Abs(points[index].y) > maxAmplitude)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        private sealed class ComplexityCondition : MissionCondition
        {
            private readonly int maxComplexity;

            public string Description => $"신호 복잡도 {maxComplexity} 이하";

            public ComplexityCondition(int maxComplexity)
            {
                this.maxComplexity = maxComplexity;
            }

            public bool IsMet(SignalPair signals, IReadOnlyList<Vector2> points)
            {
                return signals.IsValid && signals.Complexity <= maxComplexity;
            }
        }
    }
}
