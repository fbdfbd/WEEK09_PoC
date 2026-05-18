using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_CharacterDefinition_", menuName = "PoC10/SO_CharacterDefinition")]
public class SO_CharacterDefinition : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("인물의 고유 ID입니다. 저장 데이터와 룰에서 참조하므로 한 번 정하면 되도록 변경하지 않습니다. 예: merchant_suspicious, teacher_sword")]
    [SerializeField] private string _id;

    [Tooltip("화면에 표시될 인물 이름입니다.")]
    [SerializeField] private string _displayName;

    [Tooltip("인물의 짧은 호칭이나 역할명입니다. 예: 수상한 상인, 검술 교관, 귀족 영애")]
    [SerializeField] private string _title;

    [TextArea]
    [Tooltip("인물 자체의 간단한 설명입니다. 현재 상태나 특정 장소 배치 정보는 넣지 않습니다.")]
    [SerializeField] private string _description;

    [Header("Tags")]
    [Tooltip("인물의 성격이나 역할을 나타내는 태그 목록입니다. 예: 교육자, 위험인물, 귀족, 소문유포자")]
    [SerializeField] private List<string> _tags = new();

    [Header("Costs")]
    [Tooltip("육성대상이 이 인물과 접촉할 때 쌓이는 기본 피로도입니다.")]
    [SerializeField] private int _baseFatigueCost = 1;

    [Tooltip("플레이어가 이 인물을 허가/통제/수정할 때 드는 기본 개입력 비용입니다.")]
    [SerializeField] private int _baseInterventionCost = 1;

    [Header("Unlock")]
    [Tooltip("게임 시작 시 플레이어가 기본으로 알고 있는 인물인지 여부입니다.")]
    [SerializeField] private bool _isKnownByDefault = true;

    [Header("Presentation")]
    [Tooltip("인물 목록이나 버튼에 표시할 아이콘입니다.")]
    [SerializeField] private Sprite _icon;

    [Tooltip("대화나 상세 화면에 표시할 인물 이미지입니다.")]
    [SerializeField] private List<Sprite> _portraitImages = new();

    [Tooltip("인물 목록에서 표시될 정렬 순서입니다. 낮을수록 먼저 표시됩니다.")]
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
    public IReadOnlyList<Sprite> PortraitImages => _portraitImages;
    public int SortOrder => _sortOrder;
}