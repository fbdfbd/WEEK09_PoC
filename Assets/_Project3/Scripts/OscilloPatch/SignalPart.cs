using UnityEngine;

namespace Project3.OscilloPatch
{
    public sealed class SignalPart
    {
        public string Id { get; }
        public string Name { get; }
        public string ValueText { get; }
        public Color Color { get; }
        public int FrequencyAdd { get; }
        public float PhaseDegreesAdd { get; }
        public float AmplitudeAdd { get; }

        public SignalPart(
            string id,
            string name,
            string valueText,
            Color color,
            int frequencyAdd = 0,
            float phaseDegreesAdd = 0f,
            float amplitudeAdd = 0f)
        {
            Id = id;
            Name = name;
            ValueText = valueText;
            Color = color;
            FrequencyAdd = frequencyAdd;
            PhaseDegreesAdd = phaseDegreesAdd;
            AmplitudeAdd = amplitudeAdd;
        }
    }
}
