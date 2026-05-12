using System.Collections.Generic;

public class BattleState
{
    public readonly BattleActorState Player = new BattleActorState();
    public readonly BattleActorState Enemy = new BattleActorState();

    public readonly CardPile NumberDeck = new CardPile();
    public readonly CardPile NumberHand = new CardPile();
    public readonly CardPile NumberDiscard = new CardPile();

    public readonly CardPile SkillDeck = new CardPile();
    public readonly CardPile SkillHand = new CardPile();
    public readonly CardPile SkillDiscard = new CardPile();

    public readonly CardPile EnemyNumberDeck = new CardPile();
    public readonly CardPile EnemyNumberHand = new CardPile();
    public readonly CardPile EnemyNumberDiscard = new CardPile();

    public readonly CardPile EnemySkillDeck = new CardPile();
    public readonly CardPile EnemySkillHand = new CardPile();
    public readonly CardPile EnemySkillDiscard = new CardPile();

    public readonly List<SkillSlotState> SkillSlots = new List<SkillSlotState>();
    public readonly List<SkillSlotState> EnemySkillSlots = new List<SkillSlotState>();
    public readonly List<int> UsedNumbersThisTurn = new List<int>();
    public readonly List<int> EnemyUsedNumbersThisTurn = new List<int>();
    public readonly List<CardInstance> UsedNumberCardsThisTurn = new List<CardInstance>();
    public readonly List<CardInstance> EnemyUsedNumberCardsThisTurn = new List<CardInstance>();
    public readonly List<CardInstance> UsedSkillCardsThisTurn = new List<CardInstance>();
    public readonly List<CardInstance> EnemyUsedSkillCardsThisTurn = new List<CardInstance>();
    public readonly List<SynergyDefinition> EquippedSynergies = new List<SynergyDefinition>();

    public CardInstance OrderSlotNumber;
    public CardInstance EnemyOrderSlotNumber;
    public int EnemyOrderValue;
    public int TurnNumber;
    public int LastTurnUsedNumberCount;
    public int EnemyLastTurnUsedNumberCount;
    public int BonusNumberDrawNextTurn;
    public int EnemyBonusNumberDrawNextTurn;
    public bool IsFirstTurn = true;

    public int GetOrderNumberValue()
    {
        NumCardDefinition definition = GetNumDefinition(OrderSlotNumber);
        if (definition == null)
        {
            return 0;
        }

        return definition.Value;
    }

    public int GetNextNumberDrawCount()
    {
        if (IsFirstTurn)
        {
            return 5;
        }

        int drawCount = LastTurnUsedNumberCount + 1 + BonusNumberDrawNextTurn;
        return ClampNumberDrawCount(drawCount);
    }

    public int GetEnemyOrderNumberValue()
    {
        NumCardDefinition definition = GetNumDefinition(EnemyOrderSlotNumber);
        if (definition == null)
        {
            return 0;
        }

        return definition.Value;
    }

    public int GetNextEnemyNumberDrawCount()
    {
        if (IsFirstTurn)
        {
            return 5;
        }

        int drawCount = EnemyLastTurnUsedNumberCount + 1 + EnemyBonusNumberDrawNextTurn;
        return ClampNumberDrawCount(drawCount);
    }

    public List<CardInstance> GetEnemyNumberHandCards()
    {
        return EnemyNumberHand.GetCards();
    }

    public List<CardInstance> GetEnemySkillHandCards()
    {
        return EnemySkillHand.GetCards();
    }

    public void SortEnemyNumberHand()
    {
        EnemyNumberHand.SortNumberCards();
    }

    public void SortEnemySkillHand()
    {
        EnemySkillHand.SortByDisplayName();
    }

    private int ClampNumberDrawCount(int drawCount)
    {

        if (drawCount < 2)
        {
            drawCount = 2;
        }

        if (drawCount > 5)
        {
            drawCount = 5;
        }

        return drawCount;
    }

    public List<CardInstance> GetNumberHandCards()
    {
        return NumberHand.GetCards();
    }

    public List<CardInstance> GetSkillHandCards()
    {
        return SkillHand.GetCards();
    }

    public void SortNumberHand()
    {
        NumberHand.SortNumberCards();
    }

    public void SortSkillHand()
    {
        SkillHand.SortByDisplayName();
    }

    public SkillSlotState GetSkillSlot(CardInstance skillCard)
    {
        for (int i = 0; i < SkillSlots.Count; i++)
        {
            if (SkillSlots[i].SkillCard == skillCard)
            {
                return SkillSlots[i];
            }
        }

        return null;
    }

    public static NumCardDefinition GetNumDefinition(CardInstance card)
    {
        if (card == null)
        {
            return null;
        }

        return card.Definition as NumCardDefinition;
    }
}
