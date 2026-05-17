using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_LocationDefinition_", menuName = "PoC10/SO_LocationDefinition")]
public class SO_LocationDefinition : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("장소의 고유 ID입니다. 저장 데이터와 룰에서 참조하므로 한 번 정하면 되도록 변경하지 않습니다.")]
    [SerializeField] private string _id;

    [Tooltip("화면에 표시될 장소 이름입니다.")]
    [SerializeField] private string _displayName;

    [TextArea]
    [Tooltip("장소의 분위기나 간단한 설명입니다.")]
    [SerializeField] private string _description;

    [Header("Tags")]
    [Tooltip("장소의 성격을 나타내는 태그 목록입니다. 예: 학원, 귀족, 위험지역, 소문중심")]
    [SerializeField] private List<string> _tags = new();

    [Header("Costs")]
    [Tooltip("육성대상이 이 장소에 노출될 때 쌓이는 기본 피로도입니다.")]
    [SerializeField] private int _baseFatigueCost = 1;

    [Tooltip("플레이어가 이 장소를 허가/통제/수정할 때 드는 기본 개입력 비용입니다.")]
    [SerializeField] private int _baseInterventionCost = 1;

    [Header("Unlock")]
    [Tooltip("게임 시작 시 기본으로 열려 있는 장소인지 여부입니다.")]
    [SerializeField] private bool _isUnlockedByDefault = true;

    [Header("Presentation")]
    [Tooltip("장소 목록이나 버튼에 표시할 아이콘입니다.")]
    [SerializeField] private Sprite _icon;

    [Tooltip("장소 진입 화면에 표시할 배경 이미지입니다.")]
    [SerializeField] private Sprite _backgroundImage;

    [Tooltip("장소 목록에서 표시될 정렬 순서입니다. 낮을수록 먼저 표시됩니다.")]
    [SerializeField] private int _sortOrder;

    public string Id => _id;
    public string DisplayName => _displayName;
    public string Description => _description;
    public IReadOnlyList<string> Tags => _tags;

    public int BaseFatigueCost => _baseFatigueCost;
    public int BaseInterventionCost => _baseInterventionCost;

    public bool IsUnlockedByDefault => _isUnlockedByDefault;

    public Sprite Icon => _icon;
    public Sprite BackgroundImage => _backgroundImage;
    public int SortOrder => _sortOrder;
}