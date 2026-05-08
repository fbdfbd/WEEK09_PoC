using System.Collections.Generic;
using UnityEngine;

namespace Project3.OscilloPatch
{
    public sealed class FrequencyRatioCondition : MissionCondition
    {
        public string Description => "1:2 \ud3d0\uace1\uc120 \uc0dd\uc131";

        public bool IsMet(SignalPair signals, IReadOnlyList<Vector2> points)
        {
            if (!signals.IsValid)
            {
                return false;
            }

            return signals.X.Frequency == 1 && signals.Y.Frequency == 2
                || signals.X.Frequency == 2 && signals.Y.Frequency == 1;
        }
    }
}
