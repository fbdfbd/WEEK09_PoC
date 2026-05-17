using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_EducationDefinition_", menuName = "PoC10/SO_EducationDefinition")]
public class SO_EducationDefinition : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("교육의 고유 ID입니다. 저장 데이터와 룰에서 참조하므로 한 번 정하면 되도록 변경하지 않습니다. 예: sword_training, etiquette_lesson")]
    [SerializeField] private string _id;

    [Tooltip("화면에 표시될 교육 이름입니다. 예: 검술 수업, 예법 수업")]
    [SerializeField] private string _displayName;

    [Tooltip("교육의 짧은 분류명이나 별칭입니다. 예: 무예, 예절, 학문, 사교")]
    [SerializeField] private string _title;

    [TextArea]
    [Tooltip("교육 자체의 간단한 설명입니다. 현재 배치 장소, 담당 인물, 허가/통제 상태는 넣지 않습니다.")]
    [SerializeField] private string _description;

    [Header("Tags")]
    [Tooltip("교육의 성격을 나타내는 태그 목록입니다. 예: 지성, 기백, 표현, 귀족, 위험")]
    [SerializeField] private List<string> _tags = new();

    [Header("Costs")]
    [Tooltip("육성대상이 이 교육에 노출될 때 쌓이는 기본 피로도입니다.")]
    [SerializeField] private int _baseFatigueCost = 1;

    [Tooltip("플레이어가 이 교육을 허가/통제/수정할 때 드는 기본 개입력 비용입니다.")]
    [SerializeField] private int _baseInterventionCost = 1;

    [Header("Unlock")]
    [Tooltip("게임 시작 시 플레이어가 기본으로 알고 있는 교육인지 여부입니다.")]
    [SerializeField] private bool _isKnownByDefault = true;

    [Header("Presentation")]
    [Tooltip("교육 목록이나 버튼에 표시할 아이콘입니다.")]
    [SerializeField] private Sprite _icon;

    [Tooltip("교육 상세 화면이나 피드백 화면에 표시할 이미지입니다.")]
    [SerializeField] private Sprite _illustrationImage;

    [Tooltip("교육 목록에서 표시될 정렬 순서입니다. 낮을수록 먼저 표시됩니다.")]
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