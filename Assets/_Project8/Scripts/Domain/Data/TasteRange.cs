using System;

namespace Project8.Domain.Data
{
    [Serializable]
    public struct TasteRange
    {
        public float Min;
        public float Max;

        public TasteRange(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public bool Contains(float value)
        {
            return value >= Min && value <= Max;
        }
    }
}
