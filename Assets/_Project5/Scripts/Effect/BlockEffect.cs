using UnityEngine;

[CreateAssetMenu(fileName = "BlockEffect", menuName = "Project5/Effects/Block")]
public class BlockEffect : CardEffect
{
    [SerializeField] private int baseBlock;
    [SerializeField] private int numberMultiplier = 1;
    [SerializeField] private bool reserveBeforeEnemyAttack = true;

    public bool ReserveBeforeEnemyAttack
    {
        get { return reserveBeforeEnemyAttack; }
    }

    public override void Apply(BattleContext context, SkillResolveData data)
    {
        int block = baseBlock + data.NumberSum * numberMultiplier;

        if (reserveBeforeEnemyAttack)
        {
            context.Player.AddReservedBlock(block);
        }
        else
        {
            context.Player.AddBlock(block);
        }
    }

    public void Setup(int newBaseBlock, int newNumberMultiplier, bool newReserveBeforeEnemyAttack)
    {
        baseBlock = newBaseBlock;
        numberMultiplier = newNumberMultiplier;
        reserveBeforeEnemyAttack = newReserveBeforeEnemyAttack;
    }
}
