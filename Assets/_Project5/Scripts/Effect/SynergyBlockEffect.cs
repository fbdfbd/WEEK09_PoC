using UnityEngine;

[CreateAssetMenu(fileName = "SynergyBlockEffect", menuName = "Project5/Synergy Effects/Block")]
public class SynergyBlockEffect : SynergyEffect
{
    [SerializeField] private int block;

    public override void Apply(BattleContext context)
    {
        context.Player.AddBlock(block);
    }

    public void Setup(int newBlock)
    {
        block = newBlock;
    }
}
