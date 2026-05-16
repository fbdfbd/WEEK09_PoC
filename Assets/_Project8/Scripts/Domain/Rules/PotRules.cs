using Project8.Domain.Data;
using Project8.Domain.Model;

namespace Project8.Domain.Rules
{
    public static class PotRules
    {
        public static void Simulate(
            PotRuntimeModel pot,
            float deltaTime,
            float thickIncreasePerSecond,
            float volumeDecreasePerSecond)
        {
            var taste = pot.Taste;
            taste.Thick += thickIncreasePerSecond * deltaTime;

            pot.SetTaste(TasteMath.ClampTaste(taste));
            pot.SetVolume(TasteMath.ClampVolume(
                pot.Volume - volumeDecreasePerSecond * deltaTime,
                pot.MaxVolume));
        }

        public static IngredientApplyResult ApplyIngredient(
            PotRuntimeModel pot,
            IngredientRuntimeModel ingredient,
            float friedRiceThreshold)
        {
            if (ingredient.IsRice)
            {
                return TryConvertToFriedRice(pot, ingredient.Id, friedRiceThreshold);
            }

            var nextTaste = TasteMath.Add(pot.Taste, ingredient.TasteDelta);
            var nextVolume = pot.Volume + ingredient.VolumeDelta;

            pot.SetTaste(TasteMath.ClampTaste(nextTaste));
            pot.SetVolume(TasteMath.ClampVolume(nextVolume, pot.MaxVolume));

            return IngredientApplyResult.Success(ingredient.Id);
        }

        public static bool CanServe(PotRuntimeModel pot, float minimumServingVolume)
        {
            return pot.Volume >= minimumServingVolume;
        }

        public static void ConsumeServing(PotRuntimeModel pot, float servingVolumeCost)
        {
            pot.SetVolume(TasteMath.ClampVolume(
                pot.Volume - servingVolumeCost,
                pot.MaxVolume));
        }

        private static IngredientApplyResult TryConvertToFriedRice(
            PotRuntimeModel pot,
            string ingredientId,
            float friedRiceThreshold)
        {
            if (pot.FoodType != FoodType.Tteokbokki)
            {
                return IngredientApplyResult.Fail(ingredientId, "이미 볶음밥 상태입니다.");
            }

            if (pot.Taste.Thick < friedRiceThreshold)
            {
                return IngredientApplyResult.Fail(ingredientId, "아직 충분히 졸아들지 않았습니다.");
            }

            var taste = pot.Taste;
            taste.Thick = 90f;

            pot.SetFoodType(FoodType.FriedRice);
            pot.SetTaste(TasteMath.ClampTaste(taste));
            pot.SetVolume(TasteMath.ClampVolume(pot.Volume + 15f, pot.MaxVolume));

            return IngredientApplyResult.Success(ingredientId);
        }
    }
}
