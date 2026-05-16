using System.Collections.Generic;
using Project8.Application.Services;
using Project8.Domain.Data;

namespace Project8.Infrastructure.Repositories
{
    public sealed class IngredientRepository : IIngredientRepository
    {
        private readonly SO_IngredientDefinition[] _ingredients;

        public IngredientRepository(SO_IngredientDefinition[] ingredients)
        {
            _ingredients = ingredients;
        }

        public IReadOnlyList<SO_IngredientDefinition> GetAll()
        {
            return _ingredients;
        }

        public bool TryGetById(string id, out SO_IngredientDefinition ingredient)
        {
            for (var i = 0; i < _ingredients.Length; i++)
            {
                var current = _ingredients[i];

                if (current != null && current.Id == id)
                {
                    ingredient = current;
                    return true;
                }
            }

            ingredient = null;
            return false;
        }
    }
}
