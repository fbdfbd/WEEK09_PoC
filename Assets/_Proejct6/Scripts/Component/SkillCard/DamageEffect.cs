using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Card Effect/Damage")]
public class DamageEffect : CardEffect
{
    [SerializeField] private int _baseDamage;
    [SerializeField] private int _numberMultiplier = 1;

    public override void Apply(BattleContext context, SkillResolveData data)
    {
        int damage = _baseDamage + data.NumberSum * _numberMultiplier;
        context.Enemy.TakeDamage(damage);
    }
}
