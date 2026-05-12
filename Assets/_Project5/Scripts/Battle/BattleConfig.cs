using UnityEngine;

[CreateAssetMenu(fileName = "BattleConfig", menuName = "Project5/Battle/Battle Config")]
public class BattleConfig : ScriptableObject
{
    [SerializeField] private NumCardPool numCardPool;
    [SerializeField] private SkillCardDefinition[] startingSkillDeck;
    [SerializeField] private SynergyDefinition[] startingSynergies;
    [SerializeField] private int playerMaxHp = 50;
    [SerializeField] private int enemyMaxHp = 50;
    [SerializeField] private int skillDrawCount = 3;
    [SerializeField] private int maxNumberHandCount = 5;
    [SerializeField] private int maxSkillHandCount = 10;

    public NumCardPool NumCardPool
    {
        get { return numCardPool; }
    }

    public SkillCardDefinition[] StartingSkillDeck
    {
        get { return startingSkillDeck; }
    }

    public SynergyDefinition[] StartingSynergies
    {
        get { return startingSynergies; }
    }

    public int PlayerMaxHp
    {
        get { return playerMaxHp; }
    }

    public int EnemyMaxHp
    {
        get { return enemyMaxHp; }
    }

    public int SkillDrawCount
    {
        get { return skillDrawCount; }
    }

    public int MaxNumberHandCount
    {
        get { return maxNumberHandCount; }
    }

    public int MaxSkillHandCount
    {
        get { return maxSkillHandCount; }
    }

    public void Setup(
        NumCardPool newNumCardPool,
        SkillCardDefinition[] newStartingSkillDeck,
        SynergyDefinition[] newStartingSynergies,
        int newPlayerMaxHp,
        int newEnemyMaxHp,
        int newSkillDrawCount,
        int newMaxNumberHandCount,
        int newMaxSkillHandCount)
    {
        numCardPool = newNumCardPool;
        startingSkillDeck = newStartingSkillDeck;
        startingSynergies = newStartingSynergies;
        playerMaxHp = newPlayerMaxHp;
        enemyMaxHp = newEnemyMaxHp;
        skillDrawCount = newSkillDrawCount;
        maxNumberHandCount = newMaxNumberHandCount;
        maxSkillHandCount = newMaxSkillHandCount;
    }
}
