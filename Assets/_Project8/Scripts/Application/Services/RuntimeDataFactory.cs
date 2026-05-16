using Project8.Domain.Data;
using Project8.Domain.Model;

namespace Project8.Application.Services
{
    public sealed class RuntimeDataFactory : IRuntimeDataFactory
    {
        private int _orderInstanceNumber;

        public PotRuntimeModel CreatePot(SO_GameConfig config)
        {
            return new PotRuntimeModel(
                config.InitialFoodType,
                config.InitialTaste,
                config.InitialVolume,
                config.MaxVolume);
        }

        public IngredientRuntimeModel CreateIngredient(SO_IngredientDefinition definition)
        {
            return new IngredientRuntimeModel(
                definition.Id,
                definition.DisplayName,
                definition.TasteDelta,
                definition.VolumeDelta,
                definition.IsRice);
        }

        public OrderRuntimeModel CreateOrder(SO_OrderDefinition definition)
        {
            _orderInstanceNumber++;

            return new OrderRuntimeModel(
                CreateOrderInstanceId(definition.Id),
                definition.Id,
                definition.DisplayName,
                definition.FoodType,
                definition.SpicyRange,
                definition.SweetRange,
                definition.ThickRange,
                definition.PatienceSeconds,
                definition.BaseScore);
        }

        private string CreateOrderInstanceId(string definitionId)
        {
            return definitionId + "_" + _orderInstanceNumber;
        }
    }
}
