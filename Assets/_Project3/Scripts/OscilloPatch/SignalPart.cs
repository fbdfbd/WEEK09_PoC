using UnityEngine;

namespace Project3.OscilloPatch
{
    public enum SignalPartKind
    {
        Oscillator,
        Amplifier,
        Attenuator,
        PhaseCoil,
        Inverter,
        Clipper,
        Filter,
        Splitter
    }

    public sealed class SignalPart
    {
        public string Id { get; }
        public string Name { get; }
        public string ValueText { get; }
        public Color Color { get; }
        public SignalPartKind Kind { get; }
        public int FrequencyAdd { get; }
        public float PhaseDegreesAdd { get; }
        public float AmplitudeAdd { get; }
        public float ClipLimit { get; }
        public float NoiseAdd { get; }
        public float HarmonicAdd { get; }
        public int ComplexityAdd { get; }

        public SignalPart(
            string id,
            string name,
            string valueText,
            Color color,
            SignalPartKind kind,
            int frequencyAdd = 0,
            float phaseDegreesAdd = 0f,
            float amplitudeAdd = 0f,
            float clipLimit = 0f,
            float noiseAdd = 0f,
            float harmonicAdd = 0f,
            int complexityAdd = 1)
        {
            Id = id;
            Name = name;
            ValueText = valueText;
            Color = color;
            Kind = kind;
            FrequencyAdd = frequencyAdd;
            PhaseDegreesAdd = phaseDegreesAdd;
            AmplitudeAdd = amplitudeAdd;
            ClipLimit = clipLimit;
            NoiseAdd = noiseAdd;
            HarmonicAdd = harmonicAdd;
            ComplexityAdd = complexityAdd;
        }
    }
}
