using System;
using UnityEngine;

namespace App.Gameplay.Effects
{
    [Serializable]
    public sealed class EffectDefinition
    {
        [SerializeField] private EffectType type;
        [SerializeField] private string targetId;
        [SerializeField] private int value;

        public EffectType Type => type;
        public string TargetId => targetId;
        public int Value => value;
    }
}
