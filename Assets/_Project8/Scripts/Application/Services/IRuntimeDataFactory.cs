using Project8.Domain.Data;
using Project8.Domain.Model;

namespace Project8.Application.Services
{
    public interface IRuntimeDataFactory
    {
        PotRuntimeModel CreatePot(SO_GameConfig config);
        IngredientRuntimeModel CreateIngredient(SO_IngredientDefinition definition);
        OrderRuntimeModel CreateOrder(SO_OrderDefinition definition);
    }
}
