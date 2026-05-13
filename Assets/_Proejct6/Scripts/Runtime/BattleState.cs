using System.Collections.Generic;

public enum BattleResult
{
    None,
    Victory,
    Defeat
}

public class BattleState
{
    public BattleActorState Player { get; private set; } = new BattleActorState();
    public BattleActorState Enemy { get; private set; } = new BattleActorState();
    public BattleLog Log { get; private set; } = new BattleLog();

    public CardPile NumberDeck { get; private set; } = new CardPile();
    public CardPile NumberHand { get; private set; } = new CardPile();
    public CardPile NumberDiscard { get; private set; } = new CardPile();

    public CardPile SkillDeck { get; private set; } = new CardPile();
    public CardPile SkillHand { get; private set; } = new CardPile();
    public CardPile SkillDiscard { get; private set; } = new CardPile();

    public CardPile EnemyNumberDeck { get; private set; } = new CardPile();
    public CardPile EnemyNumberHand { get; private set; } = new CardPile();
    public CardPile EnemyNumberDiscard { get; private set; } = new CardPile();

    public CardPile EnemySkillDeck { get; private set; } = new CardPile();
    public CardPile EnemySkillHand { get; private set; } = new CardPile();
    public CardPile EnemySkillDiscard { get; private set; } = new CardPile();

    public CardInstance OrderSlotNumber { get; set; }
    public CardInstance EnemyOrderSlotNumber { get; set; }
    public List<SkillSlotState> SkillSlots { get; private set; } = new List<SkillSlotState>();
    public List<SkillSlotState> EnemySkillSlots { get; private set; } = new List<SkillSlotState>();

    public List<int> UsedNumbersThisTurn { get; private set; } = new List<int>();
    public List<int> EnemyUsedNumbersThisTurn { get; private set; } = new List<int>();
    public List<CardInstance> UsedNumberCardsThisTurn { get; private set; } = new List<CardInstance>();
    public List<CardInstance> EnemyUsedNumberCardsThisTurn { get; private set; } = new List<CardInstance>();
    public List<CardInstance> UsedSkillCardsThisTurn { get; private set; } = new List<CardInstance>();
    public List<CardInstance> EnemyUsedSkillCardsThisTurn { get; private set; } = new List<CardInstance>();
    public List<SynergyDefinition> EquippedSynergies { get; private set; } = new List<SynergyDefinition>();
    public List<SynergyPopupRequest> PendingSynergyPopups { get; private set; } = new List<SynergyPopupRequest>();

    public int TurnNumber { get; set; }
    public int LastTurnUsedNumberCount { get; set; }
    public int LastTurnUsedSkillCount { get; set; }
    public int EnemyLastTurnUsedNumberCount { get; set; }
    public int BonusNumberDrawNextTurn { get; set; }
    public bool IsFirstTurn { get; set; } = true;
    public bool HasUsedExtraDrawThisTurn { get; set; }
    public BattleResult Result { get; set; }
    public bool IsBattleFinished
    {
        get { return Result != BattleResult.None; }
    }

    public int GetOrderValue()
    {
        NumCardDefinition definition = GetNumDefinition(OrderSlotNumber);
        if (definition == null)
        {
            return 0;
        }

        return definition.Number;
    }

    public int GetNextNumberDrawCount()
    {
        if (IsFirstTurn)
        {
            return 5;
        }

        int drawCount = LastTurnUsedNumberCount + 1 + BonusNumberDrawNextTurn;

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

    public int GetEnemyOrderValue()
    {
        NumCardDefinition definition = GetNumDefinition(EnemyOrderSlotNumber);
        if (definition == null)
        {
            return 0;
        }

        return definition.Number;
    }

    public int GetNextEnemyNumberDrawCount()
    {
        if (IsFirstTurn)
        {
            return 5;
        }

        int drawCount = EnemyLastTurnUsedNumberCount + 1;

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

    public static SkillCardDefinition GetSkillDefinition(CardInstance card)
    {
        if (card == null)
        {
            return null;
        }

        return card.Definition as SkillCardDefinition;
    }
}
