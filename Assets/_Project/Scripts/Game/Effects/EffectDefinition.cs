using System;
using UnityEngine;

namespace App.Gameplay.Effects
{
    [Serializable]
    public sealed class EffectDefinition
    {
        [SerializeField] private EffectType _type;
        [SerializeField] private string _targetId;
        [SerializeField] private App.Gameplay.Runtime.CharacterStatType _statType;
        [SerializeField] private int _value;
        [SerializeField] private string _title;
        [SerializeField, TextArea(2, 5)] private string _body;

        public EffectType Type => _type;
        public string TargetId => _targetId;
        public App.Gameplay.Runtime.CharacterStatType StatType => _statType;
        public int Value => _value;
        public string Title => _title;
        public string Body => _body;
    }
}
