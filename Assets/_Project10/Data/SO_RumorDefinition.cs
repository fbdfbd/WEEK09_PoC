using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_RumorDefinition_", menuName = "PoC10/SO_RumorDefinition")]
public class SO_RumorDefinition : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("소문의 고유 ID입니다. 저장 데이터와 룰에서 참조하므로 한 번 정하면 되도록 변경하지 않습니다. 예: noble_support_rumor, harbor_smuggler_rumor")]
    [SerializeField] private string _id;

    [Tooltip("화면에 표시될 소문 이름입니다. 예: 귀족 후원 소문, 항구 밀수 소문")]
    [SerializeField] private string _displayName;

    [Tooltip("소문의 짧은 분류명이나 별칭입니다. 예: 평판, 위험, 연애, 학원")]
    [SerializeField] private string _title;

    [TextArea]
    [Tooltip("소문 자체의 내용입니다. 현재 유통 상태, 수정 여부, 차단 여부는 넣지 않습니다.")]
    [SerializeField] private string _description;

    [Header("Tags")]
    [Tooltip("소문의 성격을 나타내는 태그 목록입니다. 예: 평판, 귀족, 위험, 연애, 학원, 항구")]
    [SerializeField] private List<string> _tags = new();

    [Header("Costs")]
    [Tooltip("육성대상이 이 소문에 노출될 때 쌓이는 기본 피로도입니다.")]
    [SerializeField] private int _baseFatigueCost = 1;

    [Tooltip("플레이어가 이 소문을 허가/통제/수정할 때 드는 기본 개입력 비용입니다.")]
    [SerializeField] private int _baseInterventionCost = 1;

    [Header("Unlock")]
    [Tooltip("게임 시작 시 플레이어가 기본으로 알고 있는 소문인지 여부입니다.")]
    [SerializeField] private bool _isKnownByDefault = true;

    [Header("Presentation")]
    [Tooltip("소문 목록이나 버튼에 표시할 아이콘입니다.")]
    [SerializeField] private Sprite _icon;

    [Tooltip("소문 상세 화면이나 피드백 화면에 표시할 이미지입니다.")]
    [SerializeField] private Sprite _illustrationImage;

    [Tooltip("소문 목록에서 표시될 정렬 순서입니다. 낮을수록 먼저 표시됩니다.")]
    [SerializeField] private int _sortOrder;

    public string Id => _id;
    public string DisplayName => _displayName;
    public string Title => _title;
    public string Description => _description;
    public IReadOnlyList<string> Tags => _tags;

    public int BaseFatigueCost => _baseFatigueCost;
    public int BaseInterventionCost => _baseInterventionCost;

    public bool IsKnownByDefault => _isKnownByDefault;

    public Sprite Icon => _icon;
    public Sprite IllustrationImage => _illustrationImage;
    public int SortOrder => _sortOrder;
}