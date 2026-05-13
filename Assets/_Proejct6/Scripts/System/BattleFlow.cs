using System;
using System.Collections.Generic;

public class BattleFlow
{
    private readonly BattleState state = new BattleState();
    private readonly BattleContext context;

    private readonly NumberDeckBuilder numberDeckBuilder = new NumberDeckBuilder();
    private readonly SkillDeckBuilder skillDeckBuilder = new SkillDeckBuilder();
    private readonly DrawSystem drawSystem = new DrawSystem();
    private readonly PlacementSystem placementSystem = new PlacementSystem();
    private readonly SkillResolveSystem skillResolveSystem = new SkillResolveSystem();
    private readonly TurnEndSystem turnEndSystem = new TurnEndSystem();

    private int skillDrawCount = 3;

    public event Action BattleChanged;
    public event Action TurnStarted;
    public event Action TurnExecuted;
    public event Action TurnEnded;

    public BattleState State => state;

    public BattleFlow()
    {
        context = new BattleContext(state);
    }

    public void Setup(
        NumCardPool numCardPool,
        IReadOnlyList<SkillCardDefinition> startingSkillDeck,
        int playerMaxHp,
        int enemyMaxHp,
        int newSkillDrawCount)
    {
        skillDrawCount = newSkillDrawCount;

        state.Player.Setup(playerMaxHp);
        state.Enemy.Setup(enemyMaxHp);

        numberDeckBuilder.BuildDefaultNumberDeck(state, numCardPool);
        skillDeckBuilder.BuildSkillDeck(state, startingSkillDeck);

        NotifyBattleChanged();
    }

    public void Setup(
        NumCardPool numCardPool,
        SkillCardPool skillCardPool,
        int playerMaxHp,
        int enemyMaxHp,
        int newSkillDrawCount)
    {
        IReadOnlyList<SkillCardDefinition> skills = null;

        if (skillCardPool != null)
        {
            skills = skillCardPool.Cards;
        }

        Setup(numCardPool, skills, playerMaxHp, enemyMaxHp, newSkillDrawCount);
    }

    public void StartTurn()
    {
        state.Player.ResetBlock();
        state.Enemy.ResetBlock();

        drawSystem.DrawNumbers(state);
        drawSystem.DrawSkills(state, skillDrawCount);

        NotifyTurnStarted();
        NotifyBattleChanged();
    }

    public void ExecuteTurn()
    {
        skillResolveSystem.ResolveBeforeEnemyAttack(context);
        skillResolveSystem.ResolveNormal(context);
        skillResolveSystem.ResolveAfterSkillResolve(context);

        NotifyTurnExecuted();

        turnEndSystem.EndTurn(state);

        NotifyTurnEnded();
        NotifyBattleChanged();
    }

    public bool PlaceNumberInOrderSlot(CardInstance numberCard)
    {
        bool result = placementSystem.PlaceNumberInOrderSlot(state, numberCard);
        NotifyBattleChanged();
        return result;
    }

    public bool ReturnOrderSlotNumber()
    {
        bool result = placementSystem.ReturnOrderSlotNumber(state);
        NotifyBattleChanged();
        return result;
    }

    public bool PlaceSkillCard(CardInstance skillCard)
    {
        bool result = placementSystem.PlaceSkillCard(state, skillCard);
        NotifyBattleChanged();
        return result;
    }

    public bool PlaceNumberOnSkill(CardInstance skillCard, CardInstance numberCard)
    {
        bool result = placementSystem.PlaceNumberOnSkill(state, skillCard, numberCard);
        NotifyBattleChanged();
        return result;
    }

    public bool ReturnNumberFromSkill(CardInstance skillCard, CardInstance numberCard)
    {
        bool result = placementSystem.ReturnNumberFromSkill(state, skillCard, numberCard);
        NotifyBattleChanged();
        return result;
    }

    public IReadOnlyList<CardInstance> GetNumberHandCards()
    {
        return state.NumberHand.GetCards();
    }

    public IReadOnlyList<CardInstance> GetSkillHandCards()
    {
        return state.SkillHand.GetCards();
    }

    public IReadOnlyList<SkillSlotState> GetSkillSlots()
    {
        return state.SkillSlots.AsReadOnly();
    }

    public CardInstance GetOrderSlotNumber()
    {
        return state.OrderSlotNumber;
    }

    public int GetPreviewNextNumberDrawCount()
    {
        int usedNumberCount = 0;

        if (state.OrderSlotNumber != null)
        {
            usedNumberCount++;
        }

        for (int i = 0; i < state.SkillSlots.Count; i++)
        {
            usedNumberCount += state.SkillSlots[i].NumberCards.Count;
        }

        int drawCount = usedNumberCount + 1;

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

    private void NotifyBattleChanged()
    {
        if (BattleChanged != null)
        {
            BattleChanged();
        }
    }

    private void NotifyTurnStarted()
    {
        if (TurnStarted != null)
        {
            TurnStarted();
        }
    }

    private void NotifyTurnExecuted()
    {
        if (TurnExecuted != null)
        {
            TurnExecuted();
        }
    }

    private void NotifyTurnEnded()
    {
        if (TurnEnded != null)
        {
            TurnEnded();
        }
    }
}
