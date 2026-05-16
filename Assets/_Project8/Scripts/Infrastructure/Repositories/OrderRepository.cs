using System.Collections.Generic;
using Project8.Application.Services;
using Project8.Domain.Data;

namespace Project8.Infrastructure.Repositories
{
    public sealed class OrderRepository : IOrderRepository
    {
        private readonly SO_OrderDefinition[] _orders;

        public OrderRepository(SO_OrderDefinition[] orders)
        {
            _orders = orders;
        }

        public IReadOnlyList<SO_OrderDefinition> GetAll()
        {
            return _orders;
        }

        public bool TryGetById(string id, out SO_OrderDefinition order)
        {
            for (var i = 0; i < _orders.Length; i++)
            {
                var current = _orders[i];

                if (current != null && current.Id == id)
                {
                    order = current;
                    return true;
                }
            }

            order = null;
            return false;
        }
    }
}
