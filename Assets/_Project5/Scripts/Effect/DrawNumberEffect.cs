using UnityEngine;

[CreateAssetMenu(fileName = "DrawNumberEffect", menuName = "Project5/Effects/Draw Number Next Turn")]
public class DrawNumberEffect : CardEffect
{
    [SerializeField] private int bonusDrawCount = 1;

    public override void Apply(BattleContext context, SkillResolveData data)
    {
        context.State.BonusNumberDrawNextTurn += bonusDrawCount;
    }
}
