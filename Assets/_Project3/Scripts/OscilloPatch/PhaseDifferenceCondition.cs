using System.Collections.Generic;
using UnityEngine;

namespace Project3.OscilloPatch
{
    public sealed class PhaseDifferenceCondition : MissionCondition
    {
        public string Description => "\uc911\uc559 \uad50\ucc28 \uc0dd\uc131";

        public bool IsMet(SignalPair signals, IReadOnlyList<Vector2> points)
        {
            if (!signals.IsValid)
            {
                return false;
            }

            float phaseDiff = Mathf.Abs(signals.Y.PhaseDegrees - signals.X.PhaseDegrees);
            phaseDiff = Mathf.Repeat(phaseDiff, 180f);
            return phaseDiff > 10f && phaseDiff < 170f;
        }
    }
}
