using System.Collections.Generic;
using Project8.Domain.Data;

namespace Project8.Application.Services
{
    public interface IIngredientRepository
    {
        IReadOnlyList<SO_IngredientDefinition> GetAll();
        bool TryGetById(string id, out SO_IngredientDefinition ingredient);
    }
}
