using UnityEngine;

[CreateAssetMenu(fileName = "SynergyBonusNumberDrawEffect", menuName = "Synergy/Effect/Bonus Number Draw")]
public class SynergyBonusNumberDrawEffect : SynergyEffect
{
    [SerializeField] private int _bonusDrawCount = 1;

    public override void Apply(BattleContext context)
    {
        context.State.BonusNumberDrawNextTurn += _bonusDrawCount;
    }
}
