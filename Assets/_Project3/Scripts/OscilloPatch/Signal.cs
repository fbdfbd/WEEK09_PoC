using UnityEngine;

namespace Project3.OscilloPatch
{
    public sealed class Signal
    {
        public int Frequency { get; private set; }
        public float PhaseDegrees { get; private set; }
        public float Amplitude { get; private set; }

        public bool HasOscillator => Frequency > 0;

        public Signal()
        {
            Frequency = 0;
            PhaseDegrees = 0f;
            Amplitude = 100f;
        }

        public void Apply(SignalPart part)
        {
            Frequency += part.FrequencyAdd;
            PhaseDegrees += part.PhaseDegreesAdd;
            Amplitude += part.AmplitudeAdd;
        }

        public float PhaseRadians => PhaseDegrees * Mathf.Deg2Rad;
    }
}
