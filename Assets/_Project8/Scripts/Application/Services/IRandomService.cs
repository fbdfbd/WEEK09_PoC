namespace Project8.Application.Services
{
    public interface IRandomService
    {
        int Range(int minInclusive, int maxExclusive);
        float Range(float minInclusive, float maxInclusive);
    }
}
