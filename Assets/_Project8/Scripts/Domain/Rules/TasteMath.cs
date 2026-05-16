using Project8.Domain.Data;
using UnityEngine;

namespace Project8.Domain.Rules
{
    public static class TasteMath
    {
        public static TasteValue Add(TasteValue left, TasteValue right)
        {
            return new TasteValue(
                left.Spicy + right.Spicy,
                left.Sweet + right.Sweet,
                left.Thick + right.Thick);
        }

        public static TasteValue ClampTaste(TasteValue taste)
        {
            return new TasteValue(
                ClampPercent(taste.Spicy),
                ClampPercent(taste.Sweet),
                ClampPercent(taste.Thick));
        }

        public static float ClampPercent(float value)
        {
            return Mathf.Clamp(value, 0f, 100f);
        }

        public static float ClampVolume(float value, float maxVolume)
        {
            return Mathf.Clamp(value, 0f, maxVolume);
        }

        public static float GetRangeScore(float value, TasteRange range)
        {
            if (range.Contains(value))
            {
                return 100f;
            }

            var distance = GetDistanceFromRange(value, range);
            return Mathf.Clamp(100f - distance, 0f, 100f);
        }

        private static float GetDistanceFromRange(float value, TasteRange range)
        {
            if (value < range.Min)
            {
                return range.Min - value;
            }

            return value - range.Max;
        }
    }
}
