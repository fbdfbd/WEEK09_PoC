using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Project6BattlePresenter : MonoBehaviour
{
    [SerializeField] private NumCardPool _numCardPool;
    [SerializeField] private SkillCardPool _skillCardPool;
    [SerializeField] private SynergyDefinition[] _startingSynergies;
    [SerializeField] private NumberCardHandView _numberCardHandView;
    [SerializeField] private SkillHandScrollView _skillHandScrollView;
    [SerializeField] private OrderNumberSlotView _orderNumberSlotView;
    [SerializeField] private BattleActorStatusView _playerStatusView;
    [SerializeField] private BattleActorStatusView _enemyStatusView;
    [SerializeField] private int _playerMaxHp = 50;
    [SerializeField] private int _enemyMaxHp = 50;
    [SerializeField] private int _skillDrawCount = 3;

    private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();
    private BattleFlow _battleFlow;

    public BattleFlow BattleFlow => _battleFlow;

    private void Awake()
    {
        _battleFlow = new BattleFlow();
        _battleFlow.BattleChanged += RefreshViews;

        if (_numberCardHandView != null)
        {
            _numberCardHandView.NumberCardDropped += OnNumberCardDropped;
        }

        if (_skillHandScrollView != null)
        {
            _skillHandScrollView.SkillCardClicked += OnSkillCardClicked;
        }

        if (_orderNumberSlotView != null)
        {
            _orderNumberSlotView.Clicked += OnOrderNumberSlotClicked;
        }
    }

    private void Start()
    {
        _battleFlow.Setup(_numCardPool, _skillCardPool, _playerMaxHp, _enemyMaxHp, _skillDrawCount);
        _battleFlow.EquipSynergies(_startingSynergies);
        _battleFlow.StartTurn();
    }

    private void OnDestroy()
    {
        if (_battleFlow != null)
        {
            _battleFlow.BattleChanged -= RefreshViews;
        }

        if (_numberCardHandView != null)
        {
            _numberCardHandView.NumberCardDropped -= OnNumberCardDropped;
        }

        if (_skillHandScrollView != null)
        {
            _skillHandScrollView.SkillCardClicked -= OnSkillCardClicked;
        }

        if (_orderNumberSlotView != null)
        {
            _orderNumberSlotView.Clicked -= OnOrderNumberSlotClicked;
        }
    }

    public void PlaceSkillCard(CardInstance skillCard)
    {
        _battleFlow.PlaceSkillCard(skillCard);
    }

    public void ExecuteTurnAndStartNextTurn()
    {
        bool executed = _battleFlow.TryExecuteTurn();
        if (executed == false)
        {
            RefreshViews();
            return;
        }

        _battleFlow.StartTurn();
    }

    public void DrawOneNumberCard()
    {
        _battleFlow.DrawOneNumberCard();
    }

    public void DrawOneSkillCard()
    {
        _battleFlow.DrawOneSkillCard();
    }

    public bool CanUseExtraDraw()
    {
        return _battleFlow.CanUseExtraDraw();
    }

    private void RefreshViews()
    {
        if (_numberCardHandView != null)
        {
            _numberCardHandView.ShowCards(_battleFlow.GetNumberHandCards());
        }

        if (_skillHandScrollView != null)
        {
            _skillHandScrollView.ShowCards(_battleFlow.GetSkillHandCards(), _battleFlow.GetSkillSlots());
        }

        if (_orderNumberSlotView != null)
        {
            _orderNumberSlotView.Show(_battleFlow.GetOrderSlotNumber());
        }

        if (_playerStatusView != null)
        {
            _playerStatusView.Show(_battleFlow.State.Player);
        }

        if (_enemyStatusView != null)
        {
            _enemyStatusView.Show(_battleFlow.State.Enemy);
        }
    }

    private void OnNumberCardDropped(CardInstance numberCard, PointerEventData eventData)
    {
        OrderNumberSlotView orderSlot = FindOrderSlotUnderPointer(eventData);
        if (orderSlot != null)
        {
            _battleFlow.PlaceNumberInOrderSlot(numberCard);
            return;
        }

        SkillCardView targetSkill = FindSkillCardUnderPointer(eventData);

        if (targetSkill == null || targetSkill.Card == null)
        {
            RefreshViews();
            return;
        }

        bool placed = _battleFlow.PlaceOrSwapNumberOnSkill(targetSkill.Card, numberCard);
        if (placed == false)
        {
            RefreshViews();
        }
    }

    private void OnSkillCardClicked(CardInstance skillCard)
    {
        _battleFlow.ReturnAllNumbersFromSkill(skillCard);
    }

    private void OnOrderNumberSlotClicked()
    {
        _battleFlow.ReturnOrderSlotNumber();
    }

    private SkillCardView FindSkillCardUnderPointer(PointerEventData eventData)
    {
        if (EventSystem.current == null)
        {
            return null;
        }

        _raycastResults.Clear();
        EventSystem.current.RaycastAll(eventData, _raycastResults);

        for (int i = 0; i < _raycastResults.Count; i++)
        {
            SkillCardView skillCardView = _raycastResults[i].gameObject.GetComponentInParent<SkillCardView>();
            if (skillCardView != null)
            {
                return skillCardView;
            }
        }

        return null;
    }

    private OrderNumberSlotView FindOrderSlotUnderPointer(PointerEventData eventData)
    {
        if (EventSystem.current == null)
        {
            return null;
        }

        _raycastResults.Clear();
        EventSystem.current.RaycastAll(eventData, _raycastResults);

        for (int i = 0; i < _raycastResults.Count; i++)
        {
            OrderNumberSlotView orderSlot = _raycastResults[i].gameObject.GetComponentInParent<OrderNumberSlotView>();
            if (orderSlot != null)
            {
                return orderSlot;
            }
        }

        return null;
    }
}
