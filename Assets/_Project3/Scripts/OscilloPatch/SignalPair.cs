namespace Project3.OscilloPatch
{
    public sealed class SignalPair
    {
        public Signal X { get; }
        public Signal Y { get; }

        public SignalPair(Signal x, Signal y)
        {
            X = x;
            Y = y;
        }

        public bool IsValid => X.HasOscillator && Y.HasOscillator;
        public float MaxAmplitude => X.Amplitude > Y.Amplitude ? X.Amplitude : Y.Amplitude;
    }
}
