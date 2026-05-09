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
        public int Complexity => X.Complexity + Y.Complexity;

        public bool HasPartKind(SignalPartKind kind)
        {
            return HasPartKind(X, kind) || HasPartKind(Y, kind);
        }

        private static bool HasPartKind(Signal signal, SignalPartKind kind)
        {
            for (int index = 0; index < signal.UsedKinds.Count; index++)
            {
                if (signal.UsedKinds[index] == kind)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
