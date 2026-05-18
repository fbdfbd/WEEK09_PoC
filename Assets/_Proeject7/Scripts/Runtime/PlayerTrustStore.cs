using R3;

public sealed class PlayerTrustStore
{
    public const int MinTrust = 0;
    public const int MaxTrust = 100;
    public const int InitialTrust = 50;

    public ReactiveProperty<int> Trust { get; } = new(InitialTrust);

    public void SetTrust(int value)
    {
        Trust.Value = Clamp(value);
    }

    public void ChangeTrust(int amount)
    {
        SetTrust(Trust.Value + amount);
    }

    private static int Clamp(int value)
    {
        if (value < MinTrust)
            return MinTrust;

        if (value > MaxTrust)
            return MaxTrust;

        return value;
    }
}
