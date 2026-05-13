using UnityEngine;

[CreateAssetMenu(fileName = "BlockEffect", menuName = "Card Effect/Block")]
public class BlockEffect : CardEffect
{
    [SerializeField] private int _baseBlock;
    [SerializeField] private int _numberMultiplier = 1;

    public override void Apply(BattleContext context, SkillResolveData data)
    {
        int block = _baseBlock + data.NumberSum * _numberMultiplier;
        context.Player.AddBlock(block);
    }
}
