using System.Collections.Generic;
using Project8.Domain.Data;

namespace Project8.Application.Services
{
    public interface IOrderRepository
    {
        IReadOnlyList<SO_OrderDefinition> GetAll();
        bool TryGetById(string id, out SO_OrderDefinition order);
    }
}
