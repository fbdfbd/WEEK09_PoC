using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Project5/Effects/Heal")]
public class HealEffect : CardEffect
{
    [SerializeField] private int baseHeal;
    [SerializeField] private int numberMultiplier = 1;

    public override void Apply(BattleContext context, SkillResolveData data)
    {
        int heal = baseHeal + data.NumberSum * numberMultiplier;
        context.Player.Heal(heal);
    }

    public void Setup(int newBaseHeal, int newNumberMultiplier)
    {
        baseHeal = newBaseHeal;
        numberMultiplier = newNumberMultiplier;
    }
}
