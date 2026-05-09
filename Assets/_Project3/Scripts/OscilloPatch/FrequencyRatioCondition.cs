using System.Collections.Generic;
using UnityEngine;

namespace Project3.OscilloPatch
{
    public sealed class FrequencyRatioCondition : MissionCondition
    {
        private readonly int xFrequency;
        private readonly int yFrequency;

        public string Description => $"{xFrequency}:{yFrequency} 폐곡선 생성";

        public FrequencyRatioCondition()
            : this(2, 3)
        {
        }

        public FrequencyRatioCondition(int xFrequency, int yFrequency)
        {
            this.xFrequency = xFrequency;
            this.yFrequency = yFrequency;
        }

        public bool IsMet(SignalPair signals, IReadOnlyList<Vector2> points)
        {
            if (!signals.IsValid)
            {
                return false;
            }

            return signals.X.Frequency == xFrequency && signals.Y.Frequency == yFrequency;
        }
    }
}
