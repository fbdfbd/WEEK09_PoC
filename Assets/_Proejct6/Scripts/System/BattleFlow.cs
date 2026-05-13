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
    private readonly SynergySystem synergySystem = new SynergySystem();
    private readonly EnemyCardSystem enemyCardSystem = new EnemyCardSystem();
    private readonly TurnOrderSystem turnOrderSystem = new TurnOrderSystem();
    private readonly TurnEndSystem turnEndSystem = new TurnEndSystem();

    private int skillDrawCount = 3;

    public event Action BattleChanged;
    public event Action TurnStarted;
    public event Action TurnExecuted;
    public event Action TurnEnded;
    public event Action<BattleResult> BattleFinished;

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
        enemyCardSystem.BuildDecks(state, numCardPool, startingSkillDeck);

        NotifyBattleChanged();
    }

    public void EquipSynergies(IReadOnlyList<SynergyDefinition> synergies)
    {
        state.EquippedSynergies.Clear();

        if (synergies == null)
        {
            NotifyBattleChanged();
            return;
        }

        for (int i = 0; i < synergies.Count; i++)
        {
            if (synergies[i] != null)
            {
                state.EquippedSynergies.Add(synergies[i]);
            }
        }

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
        if (state.IsBattleFinished)
        {
            return;
        }

        state.Player.ResetBlock();
        state.Enemy.ResetBlock();
        state.HasUsedExtraDrawThisTurn = false;

        drawSystem.DrawNumbers(state);
        drawSystem.DrawSkills(state, GetNextSkillDrawCount());
        state.BonusNumberDrawNextTurn = 0;
        enemyCardSystem.StartTurn(state);
        state.Log.Add("턴 " + (state.TurnNumber + 1) + " 시작");

        NotifyTurnStarted();
        NotifyBattleChanged();
    }

    public void ExecuteTurn()
    {
        if (state.IsBattleFinished)
        {
            return;
        }

        bool playerFirst = turnOrderSystem.IsPlayerFirst(state);
        state.Log.Add("적이 턴카드 " + state.GetEnemyOrderValue() + "을 공개했습니다.");
        state.Log.Add("플레이어 턴카드: " + state.GetOrderValue());
        state.Log.Add("적 턴카드: " + state.GetEnemyOrderValue());

        if (playerFirst)
        {
            state.Log.Add("플레이어가 먼저 행동합니다.");
            ResolvePlayerTurn();
            if (TryFinishBattle())
            {
                return;
            }

            ResolveEnemyTurn();
            if (TryFinishBattle())
            {
                return;
            }
        }
        else
        {
            state.Log.Add("적이 먼저 행동합니다.");
            ResolveEnemyTurn();
            if (TryFinishBattle())
            {
                return;
            }

            ResolvePlayerTurn();
            if (TryFinishBattle())
            {
                return;
            }
        }

        NotifyTurnExecuted();

        turnEndSystem.EndTurn(state);
        enemyCardSystem.EndTurn(state);

        NotifyTurnEnded();
        NotifyBattleChanged();
    }

    public bool CanExecuteTurn()
    {
        return state.IsBattleFinished == false && state.OrderSlotNumber != null;
    }

    public bool TryExecuteTurn()
    {
        if (CanExecuteTurn() == false)
        {
            return false;
        }

        ExecuteTurn();
        return true;
    }

    public bool PlaceNumberInOrderSlot(CardInstance numberCard)
    {
        if (state.IsBattleFinished)
        {
            return false;
        }

        bool result = placementSystem.PlaceNumberInOrderSlot(state, numberCard);
        NotifyBattleChanged();
        return result;
    }

    public bool DrawOneNumberCard()
    {
        if (state.IsBattleFinished)
        {
            return false;
        }

        if (state.HasUsedExtraDrawThisTurn)
        {
            return false;
        }

        drawSystem.DrawNumber(state);
        state.HasUsedExtraDrawThisTurn = true;
        NotifyBattleChanged();
        return true;
    }

    public bool DrawOneSkillCard()
    {
        if (state.IsBattleFinished)
        {
            return false;
        }

        if (state.HasUsedExtraDrawThisTurn)
        {
            return false;
        }

        drawSystem.DrawSkill(state);
        state.HasUsedExtraDrawThisTurn = true;
        NotifyBattleChanged();
        return true;
    }

    public bool CanUseExtraDraw()
    {
        return state.IsBattleFinished == false && state.HasUsedExtraDrawThisTurn == false;
    }

    public bool ReturnOrderSlotNumber()
    {
        if (state.IsBattleFinished)
        {
            return false;
        }

        bool result = placementSystem.ReturnOrderSlotNumber(state);
        NotifyBattleChanged();
        return result;
    }

    public bool PlaceSkillCard(CardInstance skillCard)
    {
        if (state.IsBattleFinished)
        {
            return false;
        }

        bool result = placementSystem.PlaceSkillCard(state, skillCard);
        NotifyBattleChanged();
        return result;
    }

    public bool PlaceNumberOnSkill(CardInstance skillCard, CardInstance numberCard)
    {
        if (state.IsBattleFinished)
        {
            return false;
        }

        bool result = placementSystem.PlaceNumberOnSkill(state, skillCard, numberCard);
        NotifyBattleChanged();
        return result;
    }

    public bool PlaceOrSwapNumberOnSkill(CardInstance skillCard, CardInstance numberCard)
    {
        if (state.IsBattleFinished)
        {
            return false;
        }

        bool result = placementSystem.PlaceOrSwapNumberOnSkill(state, skillCard, numberCard);
        NotifyBattleChanged();
        return result;
    }

    public bool ReturnNumberFromSkill(CardInstance skillCard, CardInstance numberCard)
    {
        if (state.IsBattleFinished)
        {
            return false;
        }

        bool result = placementSystem.ReturnNumberFromSkill(state, skillCard, numberCard);
        NotifyBattleChanged();
        return result;
    }

    public bool ReturnAllNumbersFromSkill(CardInstance skillCard)
    {
        if (state.IsBattleFinished)
        {
            return false;
        }

        bool result = placementSystem.ReturnAllNumbersFromSkill(state, skillCard);
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

    public CardInstance GetEnemyOrderSlotNumber()
    {
        return state.EnemyOrderSlotNumber;
    }

    public bool IsPlayerFirst()
    {
        return turnOrderSystem.IsPlayerFirst(state);
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

    private void ResolvePlayerTurn()
    {
        state.Log.Add("플레이어 스킬을 처리합니다.");
        skillResolveSystem.ResolveBeforeEnemyAttack(context);
        skillResolveSystem.ResolveNormal(context);
        skillResolveSystem.ResolveAfterSkillResolve(context);
        synergySystem.Resolve(context);
    }

    private void ResolveEnemyTurn()
    {
        state.Log.Add("적 스킬을 처리합니다.");
        enemyCardSystem.ResolveEnemySkills(state);
    }

    private int GetNextSkillDrawCount()
    {
        if (state.IsFirstTurn)
        {
            return skillDrawCount;
        }

        int drawCount = state.LastTurnUsedSkillCount + 1;

        if (drawCount < 1)
        {
            drawCount = 1;
        }

        if (drawCount > skillDrawCount)
        {
            drawCount = skillDrawCount;
        }

        return drawCount;
    }

    private bool TryFinishBattle()
    {
        if (state.IsBattleFinished)
        {
            return true;
        }

        if (state.Enemy.Hp <= 0)
        {
            FinishBattle(BattleResult.Victory);
            return true;
        }

        if (state.Player.Hp <= 0)
        {
            FinishBattle(BattleResult.Defeat);
            return true;
        }

        return false;
    }

    private void FinishBattle(BattleResult result)
    {
        state.Result = result;

        if (result == BattleResult.Victory)
        {
            state.Log.Add("전투 승리");
        }
        else if (result == BattleResult.Defeat)
        {
            state.Log.Add("전투 패배");
        }

        NotifyTurnExecuted();
        NotifyBattleChanged();

        if (BattleFinished != null)
        {
            BattleFinished(result);
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
