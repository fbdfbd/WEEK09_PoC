using UnityEngine;

[CreateAssetMenu(fileName = "SynergyBonusNumberDrawEffect", menuName = "Project5/Synergy Effects/Bonus Number Draw")]
public class SynergyBonusNumberDrawEffect : SynergyEffect
{
    [SerializeField] private int bonusDrawCount = 1;

    public override void Apply(BattleContext context)
    {
        context.State.BonusNumberDrawNextTurn += bonusDrawCount;
    }

    public void Setup(int newBonusDrawCount)
    {
        bonusDrawCount = newBonusDrawCount;
    }
}
