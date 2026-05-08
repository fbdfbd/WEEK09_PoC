using System.Collections.Generic;
using UnityEngine;

namespace Project2
{
    public readonly struct SparkPoint
    {
        public readonly Vector2 Position;

        public SparkPoint(Vector2 position)
        {
            Position = position;
        }
    }

    public static class Poc2LissajousMath
    {
        public static void FillPoints(
            List<Vector3> points,
            int frequencyX,
            int frequencyY,
            float phase,
            float rotation,
            float amplitude,
            int resolution)
        {
            points.Clear();

            float maxTime = Mathf.PI * 2f * Mathf.Max(frequencyX, frequencyY);
            float cos = Mathf.Cos(rotation);
            float sin = Mathf.Sin(rotation);

            for (int i = 0; i <= resolution; i++)
            {
                float time = (i / (float)resolution) * maxTime;
                float x = amplitude * Mathf.Sin(frequencyX * time);
                float y = amplitude * Mathf.Sin(frequencyY * time + phase);

                float rotatedX = x * cos - y * sin;
                float rotatedY = x * sin + y * cos;
                points.Add(new Vector3(rotatedX, rotatedY, 0f));
            }
        }

        public static void FindSparks(
            List<Vector3> redPoints,
            List<Vector3> bluePoints,
            List<SparkPoint> sparks,
            float cellSize,
            float mergeDistance)
        {
            sparks.Clear();

            Dictionary<Vector2Int, Vector2> redGrid = new Dictionary<Vector2Int, Vector2>(redPoints.Count);
            for (int i = 0; i < redPoints.Count; i++)
            {
                Vector2 point = redPoints[i];
                redGrid[GetCell(point, cellSize)] = point;
            }

            float mergeDistanceSqr = mergeDistance * mergeDistance;
            for (int i = 0; i < bluePoints.Count; i++)
            {
                Vector2 point = bluePoints[i];
                if (!redGrid.ContainsKey(GetCell(point, cellSize)))
                {
                    continue;
                }

                bool alreadyExists = false;
                for (int j = 0; j < sparks.Count; j++)
                {
                    if ((point - sparks[j].Position).sqrMagnitude < mergeDistanceSqr)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                if (!alreadyExists)
                {
                    sparks.Add(new SparkPoint(point));
                }
            }
        }

        private static Vector2Int GetCell(Vector2 point, float cellSize)
        {
            return new Vector2Int(
                Mathf.FloorToInt(point.x / cellSize),
                Mathf.FloorToInt(point.y / cellSize));
        }
    }
}
