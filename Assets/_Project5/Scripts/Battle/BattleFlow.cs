using System;
using System.Collections.Generic;

public class BattleFlow
{
    private readonly BattleState state = new BattleState();
    private readonly DrawSystem drawSystem = new DrawSystem();
    private readonly PlacementSystem placementSystem = new PlacementSystem();
    private readonly OrderSystem orderSystem = new OrderSystem();
    private readonly EnemySystem enemySystem = new EnemySystem();
    private readonly SkillResolveSystem skillResolveSystem = new SkillResolveSystem();
    private readonly SynergySystem synergySystem = new SynergySystem();
    private readonly TurnEndSystem turnEndSystem = new TurnEndSystem();

    private BattleConfig config;
    private BattleContext context;
    private EnemyIntent currentEnemyIntent;

    public event Action BattleChanged;
    public event Action TurnStarted;
    public event Action TurnExecuted;
    public event Action TurnEnded;

    public BattleState State
    {
        get { return state; }
    }

    public EnemyIntent CurrentEnemyIntent
    {
        get { return currentEnemyIntent; }
    }

    public void Setup(BattleConfig battleConfig)
    {
        config = battleConfig;
        context = new BattleContext(state);

        state.Player.Setup(config.PlayerMaxHp);
        state.Enemy.Setup(config.EnemyMaxHp);

        BuildNumberDeck();
        BuildEnemyNumberDeck();
        BuildSkillDeck();
        BuildEnemySkillDeck();
        EquipStartingSynergies();

        state.NumberDeck.Shuffle();
        state.SkillDeck.Shuffle();
        state.EnemyNumberDeck.Shuffle();
        state.EnemySkillDeck.Shuffle();

        NotifyBattleChanged();
    }

    public void StartTurn()
    {
        state.Player.ResetBlock();
        state.Enemy.ResetBlock();
        state.UsedNumbersThisTurn.Clear();
        state.UsedNumberCardsThisTurn.Clear();
        state.UsedSkillCardsThisTurn.Clear();

        int numberDrawCount = state.GetNextNumberDrawCount();
        int enemyNumberDrawCount = state.GetNextEnemyNumberDrawCount();
        state.BonusNumberDrawNextTurn = 0;
        state.EnemyBonusNumberDrawNextTurn = 0;

        drawSystem.DrawNumbers(state, numberDrawCount, config.MaxNumberHandCount);
        drawSystem.DrawSkills(state, config.SkillDrawCount, config.MaxSkillHandCount);
        drawSystem.DrawNumbersForEnemy(state, enemyNumberDrawCount, config.MaxNumberHandCount);
        drawSystem.DrawSkillsForEnemy(state, config.SkillDrawCount, config.MaxSkillHandCount);

        enemySystem.PrepareEnemyCards(state);
        currentEnemyIntent = enemySystem.CreateIntent(state);
        state.EnemyOrderValue = currentEnemyIntent.OrderValue;

        NotifyTurnStarted();
        NotifyBattleChanged();
    }

    public void ExecuteTurn()
    {
        RegisterOrderNumberAsUsed();
        skillResolveSystem.ReserveDefense(context);

        bool playerFirst = orderSystem.IsPlayerFirst(state);

        if (playerFirst)
        {
            skillResolveSystem.ResolvePlayerSkills(context);
            synergySystem.ResolveSynergies(context);
            enemySystem.ApplyIntent(context, currentEnemyIntent);
        }
        else
        {
            enemySystem.ApplyIntent(context, currentEnemyIntent);
            skillResolveSystem.ResolvePlayerSkills(context);
            synergySystem.ResolveSynergies(context);
        }

        NotifyTurnExecuted();

        turnEndSystem.EndTurn(state, enemySystem);

        NotifyTurnEnded();
        NotifyBattleChanged();
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

    public bool PlaceNumberInOrderSlot(CardInstance numberCard)
    {
        bool result = placementSystem.PlaceNumberInOrderSlot(state, numberCard);
        NotifyBattleChanged();
        return result;
    }

    public bool ReturnNumberFromOrderSlot()
    {
        bool result = placementSystem.ReturnNumberFromOrderSlot(state);
        NotifyBattleChanged();
        return result;
    }

    public List<CardInstance> GetNumberHandCards()
    {
        return state.GetNumberHandCards();
    }

    public List<CardInstance> GetSkillHandCards()
    {
        return state.GetSkillHandCards();
    }

    public List<CardInstance> GetEnemyNumberHandCards()
    {
        return state.GetEnemyNumberHandCards();
    }

    public List<CardInstance> GetEnemySkillHandCards()
    {
        return state.GetEnemySkillHandCards();
    }

    public List<SkillSlotState> GetPlacedSkillSlots()
    {
        return new List<SkillSlotState>(state.SkillSlots);
    }

    public CardInstance GetOrderSlotCard()
    {
        return state.OrderSlotNumber;
    }

    public CardInstance GetEnemyOrderSlotCard()
    {
        return state.EnemyOrderSlotNumber;
    }

    public int GetPreviewNextNumberDrawCount()
    {
        if (state.IsFirstTurn)
        {
            return 5;
        }

        int usedCount = state.UsedNumbersThisTurn.Count;

        if (state.OrderSlotNumber != null)
        {
            usedCount++;
        }

        for (int i = 0; i < state.SkillSlots.Count; i++)
        {
            usedCount += state.SkillSlots[i].NumberCards.Count;
        }

        int drawCount = usedCount + 1 + state.BonusNumberDrawNextTurn;

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

    public bool WillPlayerActFirst()
    {
        return orderSystem.IsPlayerFirst(state);
    }

    public void SortNumberHand()
    {
        state.SortNumberHand();
        NotifyBattleChanged();
    }

    public void SortSkillHand()
    {
        state.SortSkillHand();
        NotifyBattleChanged();
    }

    public void SortEnemyNumberHand()
    {
        state.SortEnemyNumberHand();
        NotifyBattleChanged();
    }

    public void SortEnemySkillHand()
    {
        state.SortEnemySkillHand();
        NotifyBattleChanged();
    }

    private void BuildNumberDeck()
    {
        if (config.NumCardPool == null)
        {
            return;
        }

        for (int value = 1; value <= 10; value++)
        {
            NumCardDefinition definition = config.NumCardPool.GetCard(value);
            AddNumberCardTwice(definition);
        }
    }

    private void AddNumberCardTwice(NumCardDefinition definition)
    {
        if (definition == null)
        {
            return;
        }

        state.NumberDeck.Add(new CardInstance(definition));
        state.NumberDeck.Add(new CardInstance(definition));
    }

    private void BuildSkillDeck()
    {
        if (config.StartingSkillDeck == null)
        {
            return;
        }

        for (int i = 0; i < config.StartingSkillDeck.Length; i++)
        {
            if (config.StartingSkillDeck[i] != null)
            {
                state.SkillDeck.Add(new CardInstance(config.StartingSkillDeck[i]));
            }
        }
    }

    private void BuildEnemyNumberDeck()
    {
        if (config.NumCardPool == null)
        {
            return;
        }

        for (int value = 1; value <= 10; value++)
        {
            NumCardDefinition definition = config.NumCardPool.GetCard(value);
            AddEnemyNumberCardTwice(definition);
        }
    }

    private void AddEnemyNumberCardTwice(NumCardDefinition definition)
    {
        if (definition == null)
        {
            return;
        }

        state.EnemyNumberDeck.Add(new CardInstance(definition));
        state.EnemyNumberDeck.Add(new CardInstance(definition));
    }

    private void BuildEnemySkillDeck()
    {
        if (config.StartingSkillDeck == null)
        {
            return;
        }

        for (int i = 0; i < config.StartingSkillDeck.Length; i++)
        {
            if (config.StartingSkillDeck[i] != null)
            {
                state.EnemySkillDeck.Add(new CardInstance(config.StartingSkillDeck[i]));
            }
        }
    }

    private void EquipStartingSynergies()
    {
        if (config.StartingSynergies == null)
        {
            return;
        }

        for (int i = 0; i < config.StartingSynergies.Length; i++)
        {
            if (config.StartingSynergies[i] != null)
            {
                state.EquippedSynergies.Add(config.StartingSynergies[i]);
            }
        }
    }

    private void RegisterOrderNumberAsUsed()
    {
        NumCardDefinition definition = BattleState.GetNumDefinition(state.OrderSlotNumber);
        if (definition == null)
        {
            return;
        }

        state.UsedNumbersThisTurn.Add(definition.Value);
        state.UsedNumberCardsThisTurn.Add(state.OrderSlotNumber);
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
