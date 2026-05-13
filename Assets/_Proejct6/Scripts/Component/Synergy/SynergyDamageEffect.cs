using UnityEngine;

[CreateAssetMenu(fileName = "SynergyDamageEffect", menuName = "Synergy/Effect/Damage")]
public class SynergyDamageEffect : SynergyEffect
{
    [SerializeField] private int _damage = 8;

    public override void Apply(BattleContext context)
    {
        context.Enemy.TakeDamage(_damage);
    }
}
