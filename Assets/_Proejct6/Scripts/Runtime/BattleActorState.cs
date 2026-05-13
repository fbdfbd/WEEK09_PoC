public class BattleActorState
{
    public int Hp { get; private set; }
    public int MaxHp { get; private set; }
    public int Block { get; private set; }

    public void Setup(int maxHp)
    {
        MaxHp = maxHp;
        Hp = maxHp;
        Block = 0;
    }

    public void ResetBlock()
    {
        Block = 0;
    }

    public void AddBlock(int value)
    {
        if (value <= 0)
        {
            return;
        }

        Block += value;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        int remainingDamage = damage;

        if (Block > 0)
        {
            int blockedDamage = Block;
            if (blockedDamage > remainingDamage)
            {
                blockedDamage = remainingDamage;
            }

            Block -= blockedDamage;
            remainingDamage -= blockedDamage;
        }

        Hp -= remainingDamage;
        if (Hp < 0)
        {
            Hp = 0;
        }
    }

    public void Heal(int value)
    {
        if (value <= 0)
        {
            return;
        }

        Hp += value;
        if (Hp > MaxHp)
        {
            Hp = MaxHp;
        }
    }
}
