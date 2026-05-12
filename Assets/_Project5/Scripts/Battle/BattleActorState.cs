public class BattleActorState
{
    private int hp;
    private int maxHp;
    private int block;
    private int reservedBlock;

    public int Hp
    {
        get { return hp; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Block
    {
        get { return block; }
    }

    public int ReservedBlock
    {
        get { return reservedBlock; }
    }

    public void Setup(int newMaxHp)
    {
        maxHp = newMaxHp;
        hp = newMaxHp;
        block = 0;
        reservedBlock = 0;
    }

    public void ResetBlock()
    {
        block = 0;
        reservedBlock = 0;
    }

    public void AddBlock(int value)
    {
        if (value <= 0)
        {
            return;
        }

        block += value;
    }

    public void AddReservedBlock(int value)
    {
        if (value <= 0)
        {
            return;
        }

        reservedBlock += value;
    }

    public void ActivateReservedBlock()
    {
        block += reservedBlock;
        reservedBlock = 0;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0)
        {
            return;
        }

        int remainingDamage = damage;

        if (block > 0)
        {
            int blockedDamage = block;
            if (blockedDamage > remainingDamage)
            {
                blockedDamage = remainingDamage;
            }

            block -= blockedDamage;
            remainingDamage -= blockedDamage;
        }

        hp -= remainingDamage;
        if (hp < 0)
        {
            hp = 0;
        }
    }

    public void Heal(int value)
    {
        if (value <= 0)
        {
            return;
        }

        hp += value;
        if (hp > maxHp)
        {
            hp = maxHp;
        }
    }
}
