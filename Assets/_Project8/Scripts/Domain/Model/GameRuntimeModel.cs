using System.Collections.Generic;

namespace Project8.Domain.Model
{
    public sealed class GameRuntimeModel
    {
        private readonly List<OrderRuntimeModel> _orders = new List<OrderRuntimeModel>();

        public PotRuntimeModel Pot { get; private set; }
        public IReadOnlyList<OrderRuntimeModel> Orders { get { return _orders; } }
        public int Score { get; private set; }
        public float RemainingSeconds { get; private set; }
        public bool IsPlaying { get; private set; }

        public void SetPot(PotRuntimeModel pot)
        {
            Pot = pot;
        }

        public void SetScore(int score)
        {
            Score = score;
        }

        public void SetRemainingSeconds(float seconds)
        {
            RemainingSeconds = seconds;
        }

        public void SetPlaying(bool isPlaying)
        {
            IsPlaying = isPlaying;
        }

        public void AddOrder(OrderRuntimeModel order)
        {
            _orders.Add(order);
        }

        public void RemoveOrder(OrderRuntimeModel order)
        {
            _orders.Remove(order);
        }

        public bool TryGetOrder(string orderInstanceId, out OrderRuntimeModel order)
        {
            for (var i = 0; i < _orders.Count; i++)
            {
                var current = _orders[i];

                if (current.InstanceId == orderInstanceId)
                {
                    order = current;
                    return true;
                }
            }

            order = null;
            return false;
        }

        public void ClearOrders()
        {
            _orders.Clear();
        }
    }
}
