using System.Collections.Generic;
using UnityEngine;

namespace Project3.OscilloPatch
{
    public sealed class LissajousPointMaker
    {
        private const int PointCount = 960;

        public List<Vector2> MakePoints(SignalPair signals)
        {
            List<Vector2> points = new List<Vector2>(PointCount + 1);

            if (!signals.IsValid)
            {
                return points;
            }

            float maxTime = Mathf.PI * 2f;

            for (int index = 0; index <= PointCount; index++)
            {
                float time = index / (float)PointCount * maxTime;
                float x = signals.X.Evaluate(time);
                float y = signals.Y.Evaluate(time);
                points.Add(new Vector2(x, y));
            }

            return points;
        }
    }
}
