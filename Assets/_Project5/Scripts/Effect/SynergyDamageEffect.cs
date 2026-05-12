using UnityEngine;

[CreateAssetMenu(fileName = "SynergyDamageEffect", menuName = "Project5/Synergy Effects/Damage")]
public class SynergyDamageEffect : SynergyEffect
{
    [SerializeField] private int damage;

    public override void Apply(BattleContext context)
    {
        context.Enemy.TakeDamage(damage);
    }

    public void Setup(int newDamage)
    {
        damage = newDamage;
    }
}
