using UnityEngine;

[CreateAssetMenu(fileName = "DamageEffect", menuName = "Project5/Effects/Damage")]
public class DamageEffect : CardEffect
{
    [SerializeField] private int baseDamage;
    [SerializeField] private int numberMultiplier = 1;

    public override void Apply(BattleContext context, SkillResolveData data)
    {
        int damage = baseDamage + data.NumberSum * numberMultiplier;
        context.Enemy.TakeDamage(damage);
    }

    public void Setup(int newBaseDamage, int newNumberMultiplier)
    {
        baseDamage = newBaseDamage;
        numberMultiplier = newNumberMultiplier;
    }
}
