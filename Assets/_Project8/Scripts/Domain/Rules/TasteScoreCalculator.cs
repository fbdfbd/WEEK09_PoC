using Project8.Domain.Data;
using Project8.Domain.Model;
using UnityEngine;

namespace Project8.Domain.Rules
{
    public sealed class TasteScoreCalculator
    {
        public ServeResult Calculate(
            PotRuntimeModel pot,
            OrderRuntimeModel order,
            float burnPenaltyThreshold)
        {
            if (pot.FoodType != order.FoodType)
            {
                return new ServeResult(order.InstanceId, 0f, 0, 0, false);
            }

            var spicyScore = TasteMath.GetRangeScore(pot.Taste.Spicy, order.SpicyRange);
            var sweetScore = TasteMath.GetRangeScore(pot.Taste.Sweet, order.SweetRange);
            var thickScore = TasteMath.GetRangeScore(pot.Taste.Thick, order.ThickRange);
            var tasteScore = (spicyScore + sweetScore + thickScore) / 3f;

            if (ShouldApplyBurnPenalty(pot, order, burnPenaltyThreshold))
            {
                tasteScore -= 20f;
            }

            tasteScore = TasteMath.ClampPercent(tasteScore);

            var star = CalculateStar(tasteScore);
            var finalScore = Mathf.RoundToInt(order.BaseScore * (tasteScore / 100f));
            var isSuccess = star > 0;

            return new ServeResult(
                order.InstanceId,
                tasteScore,
                star,
                finalScore,
                isSuccess);
        }

        private static bool ShouldApplyBurnPenalty(
            PotRuntimeModel pot,
            OrderRuntimeModel order,
            float burnPenaltyThreshold)
        {
            return order.FoodType == FoodType.Tteokbokki
                && pot.Taste.Thick >= burnPenaltyThreshold;
        }

        private static int CalculateStar(float tasteScore)
        {
            if (tasteScore >= 90f)
            {
                return 3;
            }

            if (tasteScore >= 70f)
            {
                return 2;
            }

            if (tasteScore >= 50f)
            {
                return 1;
            }

            return 0;
        }
    }
}
