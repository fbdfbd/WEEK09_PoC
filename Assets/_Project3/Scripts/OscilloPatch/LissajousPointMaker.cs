using System.Collections.Generic;
using UnityEngine;

namespace Project3.OscilloPatch
{
    public sealed class LissajousPointMaker
    {
        private const int PointCount = 360;

        public List<Vector2> MakePoints(SignalPair signals)
        {
            List<Vector2> points = new List<Vector2>(PointCount + 1);

            if (!signals.IsValid)
            {
                return points;
            }

            int loopCount = Mathf.Max(signals.X.Frequency, signals.Y.Frequency);
            float maxTime = Mathf.PI * 2f * loopCount;

            for (int index = 0; index <= PointCount; index++)
            {
                float time = index / (float)PointCount * maxTime;
                float x = Mathf.Sin(signals.X.Frequency * time + signals.X.PhaseRadians) * signals.X.Amplitude;
                float y = Mathf.Sin(signals.Y.Frequency * time + signals.Y.PhaseRadians) * signals.Y.Amplitude;
                points.Add(new Vector2(x, y));
            }

            return points;
        }
    }
}
