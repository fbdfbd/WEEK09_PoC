using System;
using System.Collections.Generic;
using Project8.Domain.Data;
using Project8.Domain.Model;

namespace Project8.Presentation.Views
{
    public interface IOrderQueueView
    {
        event Action<string> ServeOrderClicked;

        void SetOrders(IReadOnlyList<OrderRuntimeModel> orders);
        void PlayOrderCompleted(ServeResult result);
        void PlayOrderExpired(string orderInstanceId);
    }
}
