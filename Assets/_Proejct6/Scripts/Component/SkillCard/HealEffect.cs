using UnityEngine;

[CreateAssetMenu(fileName = "HealEffect", menuName = "Card Effect/Heal")]
public class HealEffect : CardEffect
{
    [SerializeField] private int _baseHeal;
    [SerializeField] private int _numberMultiplier = 1;

    public override void Apply(BattleContext context, SkillResolveData data)
    {
        int heal = _baseHeal + data.NumberSum * _numberMultiplier;
        context.Player.Heal(heal);
    }
}
