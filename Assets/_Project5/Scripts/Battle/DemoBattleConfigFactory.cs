using UnityEngine;

public static class DemoBattleConfigFactory
{
    public static BattleConfig Create()
    {
        NumCardPool numCardPool = CreateNumCardPool();
        SkillCardDefinition[] skillDeck = CreateSkillDeck();
        SynergyDefinition[] synergies = CreateSynergies();

        BattleConfig config = ScriptableObject.CreateInstance<BattleConfig>();
        config.Setup(
            numCardPool,
            skillDeck,
            synergies,
            50,
            50,
            3,
            5,
            10);

        return config;
    }

    private static NumCardPool CreateNumCardPool()
    {
        NumCardDefinition[] cards = new NumCardDefinition[10];

        for (int i = 0; i < cards.Length; i++)
        {
            NumCardDefinition card = ScriptableObject.CreateInstance<NumCardDefinition>();
            card.Setup(i + 1);
            cards[i] = card;
        }

        NumCardPool pool = ScriptableObject.CreateInstance<NumCardPool>();
        pool.Setup(cards);
        return pool;
    }

    private static SkillCardDefinition[] CreateSkillDeck()
    {
        SkillCardDefinition strike = CreateAttackSkill("strike", "Strike", 0, 1);
        SkillCardDefinition heavyStrike = CreateAttackSkill("heavy_strike", "Heavy Strike", 2, 2);
        SkillCardDefinition guard = CreateBlockSkill("guard", "Guard", 2, 1);
        SkillCardDefinition recover = CreateHealSkill("recover", "Recover", 1, 1);

        return new SkillCardDefinition[]
        {
            strike,
            strike,
            heavyStrike,
            guard,
            guard,
            recover
        };
    }

    private static SkillCardDefinition CreateAttackSkill(string id, string displayName, int baseDamage, int multiplier)
    {
        DamageEffect effect = ScriptableObject.CreateInstance<DamageEffect>();
        effect.Setup(baseDamage, multiplier);

        SkillCardDefinition skill = ScriptableObject.CreateInstance<SkillCardDefinition>();
        skill.Setup(id, displayName, SkillCardRole.Attack, 1, new CardEffect[] { effect });
        return skill;
    }

    private static SkillCardDefinition CreateBlockSkill(string id, string displayName, int baseBlock, int multiplier)
    {
        BlockEffect effect = ScriptableObject.CreateInstance<BlockEffect>();
        effect.Setup(baseBlock, multiplier, true);

        SkillCardDefinition skill = ScriptableObject.CreateInstance<SkillCardDefinition>();
        skill.Setup(id, displayName, SkillCardRole.Defense, 1, new CardEffect[] { effect });
        return skill;
    }

    private static SkillCardDefinition CreateHealSkill(string id, string displayName, int baseHeal, int multiplier)
    {
        HealEffect effect = ScriptableObject.CreateInstance<HealEffect>();
        effect.Setup(baseHeal, multiplier);

        SkillCardDefinition skill = ScriptableObject.CreateInstance<SkillCardDefinition>();
        skill.Setup(id, displayName, SkillCardRole.Heal, 1, new CardEffect[] { effect });
        return skill;
    }

    private static SynergyDefinition[] CreateSynergies()
    {
        SynergyDefinition manyNumbers = CreateManyNumbersSynergy();
        SynergyDefinition lowSum = CreateLowSumSynergy();
        SynergyDefinition straight = CreateStraightSynergy();

        return new SynergyDefinition[]
        {
            manyNumbers,
            lowSum,
            straight
        };
    }

    private static SynergyDefinition CreateManyNumbersSynergy()
    {
        UsedNumberCountCondition condition = ScriptableObject.CreateInstance<UsedNumberCountCondition>();
        condition.Setup(CompareType.GreaterOrEqual, 4);

        SynergyDamageEffect effect = ScriptableObject.CreateInstance<SynergyDamageEffect>();
        effect.Setup(6);

        SynergyDefinition synergy = ScriptableObject.CreateInstance<SynergyDefinition>();
        synergy.Setup("many_numbers", "Many Numbers", new SynergyCondition[] { condition }, new SynergyEffect[] { effect });
        return synergy;
    }

    private static SynergyDefinition CreateLowSumSynergy()
    {
        UsedNumberSumCondition condition = ScriptableObject.CreateInstance<UsedNumberSumCondition>();
        condition.Setup(CompareType.LessOrEqual, 10);

        SynergyBonusNumberDrawEffect effect = ScriptableObject.CreateInstance<SynergyBonusNumberDrawEffect>();
        effect.Setup(1);

        SynergyDefinition synergy = ScriptableObject.CreateInstance<SynergyDefinition>();
        synergy.Setup("low_sum", "Low Sum", new SynergyCondition[] { condition }, new SynergyEffect[] { effect });
        return synergy;
    }

    private static SynergyDefinition CreateStraightSynergy()
    {
        HasStraightCondition condition = ScriptableObject.CreateInstance<HasStraightCondition>();
        condition.Setup(3);

        SynergyDamageEffect effect = ScriptableObject.CreateInstance<SynergyDamageEffect>();
        effect.Setup(8);

        SynergyDefinition synergy = ScriptableObject.CreateInstance<SynergyDefinition>();
        synergy.Setup("straight", "Straight", new SynergyCondition[] { condition }, new SynergyEffect[] { effect });
        return synergy;
    }
}
