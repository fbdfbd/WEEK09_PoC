using System.Collections.Generic;

public class BattleState
{
    public BattleActorState Player { get; private set; } = new BattleActorState();
    public BattleActorState Enemy { get; private set; } = new BattleActorState();

    public CardPile NumberDeck { get; private set; } = new CardPile();
    public CardPile NumberHand { get; private set; } = new CardPile();
    public CardPile NumberDiscard { get; private set; } = new CardPile();

    public CardPile SkillDeck { get; private set; } = new CardPile();
    public CardPile SkillHand { get; private set; } = new CardPile();
    public CardPile SkillDiscard { get; private set; } = new CardPile();

    public CardInstance OrderSlotNumber { get; set; }
    public List<SkillSlotState> SkillSlots { get; private set; } = new List<SkillSlotState>();

    public List<int> UsedNumbersThisTurn { get; private set; } = new List<int>();
    public List<CardInstance> UsedNumberCardsThisTurn { get; private set; } = new List<CardInstance>();
    public List<CardInstance> UsedSkillCardsThisTurn { get; private set; } = new List<CardInstance>();

    public int TurnNumber { get; set; }
    public int LastTurnUsedNumberCount { get; set; }
    public bool IsFirstTurn { get; set; } = true;

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

        int drawCount = LastTurnUsedNumberCount + 1;

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
