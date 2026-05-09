using UnityEngine;

namespace Project3.OscilloPatch
{
    public sealed class Signal
    {
        public int Frequency { get; private set; }
        public float PhaseDegrees { get; private set; }
        public float Amplitude { get; private set; }
        public float ClipLimit { get; private set; }
        public float Noise { get; private set; }
        public float Harmonic { get; private set; }
        public int Complexity { get; private set; }

        public bool HasOscillator => Frequency > 0;

        public Signal()
        {
            Frequency = 0;
            PhaseDegrees = 0f;
            Amplitude = 100f;
            ClipLimit = 999f;
            Noise = 0f;
            Harmonic = 0f;
            Complexity = 0;
        }

        public void Apply(SignalPart part)
        {
            switch (part.Kind)
            {
                case SignalPartKind.Oscillator:
                    Frequency += part.FrequencyAdd;
                    break;
                case SignalPartKind.Amplifier:
                case SignalPartKind.Attenuator:
                    Amplitude = Mathf.Clamp(Amplitude + part.AmplitudeAdd, 20f, 210f);
                    break;
                case SignalPartKind.PhaseCoil:
                case SignalPartKind.Inverter:
                    PhaseDegrees += part.PhaseDegreesAdd;
                    break;
                case SignalPartKind.Clipper:
                    ClipLimit = Mathf.Min(ClipLimit, part.ClipLimit);
                    break;
                case SignalPartKind.Filter:
                    Noise = Mathf.Max(0f, Noise + part.NoiseAdd);
                    Amplitude = Mathf.Clamp(Amplitude + part.AmplitudeAdd, 20f, 210f);
                    break;
                case SignalPartKind.Splitter:
                    Harmonic += part.HarmonicAdd;
                    break;
            }

            Complexity = Mathf.Max(0, Complexity + part.ComplexityAdd);
        }

        public float PhaseRadians => PhaseDegrees * Mathf.Deg2Rad;

        public float Evaluate(float time)
        {
            float value = Mathf.Sin(Frequency * time + PhaseRadians) * Amplitude;

            if (Harmonic > 0f)
            {
                value += Mathf.Sin(Frequency * 2f * time + PhaseRadians * 0.5f) * Amplitude * Harmonic;
            }

            if (Noise > 0f)
            {
                value += Mathf.Sin(time * 17.13f + Frequency * 1.7f) * Noise;
                value += Mathf.Sin(time * 31.7f + PhaseRadians) * Noise * 0.45f;
            }

            return Mathf.Clamp(value, -ClipLimit, ClipLimit);
        }
    }
}
