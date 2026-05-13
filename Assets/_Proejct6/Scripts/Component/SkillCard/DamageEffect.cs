using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Card Effect/Damage")]
public class DamageEffect : CardEffect
{
    [SerializeField] private int _baseDamage;
    [SerializeField] private int _numberMultiplier = 1;

    public override void Apply(BattleContext context, SkillResolveData data)
    {
        int damage = CalculateDamage(data);
        context.Enemy.TakeDamage(damage);
    }

    public int CalculateDamage(SkillResolveData data)
    {
        if (data == null)
        {
            return 0;
        }

        return _baseDamage + data.NumberSum * _numberMultiplier;
    }
}
