using System.Collections.Generic;
using UnityEngine;

namespace Project3.OscilloPatch
{
    public interface MissionCondition
    {
        string Description { get; }
        bool IsMet(SignalPair signals, IReadOnlyList<Vector2> points);
    }
}
