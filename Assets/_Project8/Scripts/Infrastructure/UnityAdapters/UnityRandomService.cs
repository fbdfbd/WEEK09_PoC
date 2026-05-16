using Project8.Application.Services;
using UnityEngine;

namespace Project8.Infrastructure.UnityAdapters
{
    public sealed class UnityRandomService : IRandomService
    {
        public int Range(int minInclusive, int maxExclusive)
        {
            return Random.Range(minInclusive, maxExclusive);
        }

        public float Range(float minInclusive, float maxInclusive)
        {
            return Random.Range(minInclusive, maxInclusive);
        }
    }
}
