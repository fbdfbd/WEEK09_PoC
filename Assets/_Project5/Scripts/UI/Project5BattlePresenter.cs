using UnityEngine;

public class Project5BattlePresenter : MonoBehaviour
{
    [SerializeField] private BattleConfig battleConfig;
    [SerializeField] private CardFanView numberHandFanView;
    [SerializeField] private SkillHandScrollView skillHandScrollView;

    private BattleFlow battleFlow;

    public BattleFlow BattleFlow
    {
        get { return battleFlow; }
    }

    private void Awake()
    {
        battleFlow = new BattleFlow();
        battleFlow.BattleChanged += RefreshViews;

        if (numberHandFanView != null)
        {
            numberHandFanView.CardClicked += OnNumberCardClicked;
        }

        if (skillHandScrollView != null)
        {
            skillHandScrollView.SkillCardClicked += OnSkillCardClicked;
        }
    }

    private void Start()
    {
        if (battleConfig == null)
        {
            battleConfig = DemoBattleConfigFactory.Create();
            Debug.Log("Project5BattlePresenter is using demo battle data.");
        }

        battleFlow.Setup(battleConfig);
        battleFlow.StartTurn();
    }

    private void OnDestroy()
    {
        if (battleFlow != null)
        {
            battleFlow.BattleChanged -= RefreshViews;
        }

        if (numberHandFanView != null)
        {
            numberHandFanView.CardClicked -= OnNumberCardClicked;
        }

        if (skillHandScrollView != null)
        {
            skillHandScrollView.SkillCardClicked -= OnSkillCardClicked;
        }
    }

    public void ExecuteTurn()
    {
        battleFlow.ExecuteTurn();
        battleFlow.StartTurn();
    }

    public void SortNumberHand()
    {
        battleFlow.SortNumberHand();
    }

    public void RefreshViews()
    {
        if (numberHandFanView != null)
        {
            numberHandFanView.ShowCards(battleFlow.GetNumberHandCards());
        }

        if (skillHandScrollView != null)
        {
            skillHandScrollView.ShowCards(battleFlow.GetSkillHandCards());
        }
    }

    private void OnNumberCardClicked(CardInstance card)
    {
        // Drag/drop or selection rules can be connected here later.
        // The fan view only reports which card was clicked.
    }

    private void OnSkillCardClicked(CardInstance card)
    {
        battleFlow.PlaceSkillCard(card);
    }
}
