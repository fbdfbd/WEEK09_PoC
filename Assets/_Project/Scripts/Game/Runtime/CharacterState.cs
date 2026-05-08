using System;
using System.Collections.Generic;

namespace App.Gameplay.Runtime
{
    public sealed class CharacterState
    {
        public const int MinStatValue = 0;
        public const int MaxStatValue = 100;
        public const int DefaultStatValue = 50;

        private readonly Dictionary<CharacterStatType, int> _stats = new();

        public CharacterState()
        {
            foreach (CharacterStatType statType in Enum.GetValues(typeof(CharacterStatType)))
            {
                _stats[statType] = DefaultStatValue;
            }
        }

        public int GetStat(CharacterStatType statType)
        {
            return _stats.TryGetValue(statType, out var value) ? value : DefaultStatValue;
        }

        public void SetStat(CharacterStatType statType, int value)
        {
            _stats[statType] = Math.Clamp(value, MinStatValue, MaxStatValue);
        }

        public void AddStat(CharacterStatType statType, int value)
        {
            SetStat(statType, GetStat(statType) + value);
        }
    }
}
