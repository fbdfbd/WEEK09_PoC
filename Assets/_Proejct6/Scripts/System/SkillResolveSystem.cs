public class SkillResolveSystem
{
    private readonly NumberRequirementChecker requirementChecker = new NumberRequirementChecker();
    private readonly EffectExecutor effectExecutor = new EffectExecutor();

    public void ResolveBeforeEnemyAttack(BattleContext context)
    {
        ResolveByTiming(context, EffectTiming.BeforeEnemyAttack);
    }

    public void ResolveNormal(BattleContext context)
    {
        ResolveByTiming(context, EffectTiming.Normal);
    }

    public void ResolveAfterSkillResolve(BattleContext context)
    {
        ResolveByTiming(context, EffectTiming.AfterSkillResolve);
    }

    private void ResolveByTiming(BattleContext context, EffectTiming timing)
    {
        for (int i = 0; i < context.State.SkillSlots.Count; i++)
        {
            SkillSlotState slot = context.State.SkillSlots[i];
            ResolveSlot(context, slot, timing);
        }
    }

    private void ResolveSlot(BattleContext context, SkillSlotState slot, EffectTiming timing)
    {
        SkillCardDefinition skill = BattleState.GetSkillDefinition(slot.SkillCard);
        if (skill == null)
        {
            return;
        }

        if (requirementChecker.CanUseNumbers(skill, slot.NumberCards) == false)
        {
            return;
        }

        SkillResolveData data = new SkillResolveData(slot.SkillCard, slot.NumberCards);
        effectExecutor.Execute(skill.Effects, context, data, timing);

        if (timing == EffectTiming.Normal)
        {
            context.State.Log.Add(BuildPlayerSkillLog(skill, data));
            RegisterUsedCards(context.State, slot, data);
        }
    }

    private string BuildPlayerSkillLog(SkillCardDefinition skill, SkillResolveData data)
    {
        int damage = GetDamageAmount(skill, data);
        string log = "플레이어가 " + skill.DisplayName + "을 발동했습니다.";

        if (damage > 0)
        {
            log += " 피해 " + damage;
        }

        return log;
    }

    private int GetDamageAmount(SkillCardDefinition skill, SkillResolveData data)
    {
        int damage = 0;

        if (skill.Effects == null)
        {
            return damage;
        }

        for (int i = 0; i < skill.Effects.Count; i++)
        {
            DamageEffect damageEffect = skill.Effects[i] as DamageEffect;
            if (damageEffect == null || damageEffect.Timing != EffectTiming.Normal)
            {
                continue;
            }

            damage += damageEffect.CalculateDamage(data);
        }

        return damage;
    }

    private void RegisterUsedCards(BattleState state, SkillSlotState slot, SkillResolveData data)
    {
        if (state.UsedSkillCardsThisTurn.Contains(slot.SkillCard) == false)
        {
            state.UsedSkillCardsThisTurn.Add(slot.SkillCard);
        }

        for (int i = 0; i < slot.NumberCards.Count; i++)
        {
            if (state.UsedNumberCardsThisTurn.Contains(slot.NumberCards[i]) == false)
            {
                state.UsedNumberCardsThisTurn.Add(slot.NumberCards[i]);
            }
        }

        for (int i = 0; i < data.Numbers.Count; i++)
        {
            state.UsedNumbersThisTurn.Add(data.Numbers[i]);
        }
    }
}
