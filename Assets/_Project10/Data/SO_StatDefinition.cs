using UnityEngine;

[CreateAssetMenu(fileName = "SO_StatDefinition_", menuName = "PoC10/SO_StatDefinition")]
public class SO_StatDefinition : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("스탯의 고유 ID입니다. 저장 데이터와 룰에서 참조하므로 한 번 정하면 되도록 변경하지 않습니다. 예: intellect, fatigue")]
    [SerializeField] private string _id;

    [Tooltip("화면에 표시될 스탯 이름입니다. 예: 지성, 피로")]
    [SerializeField] private string _displayName;

    [Header("Category")]
    [Tooltip("스탯의 분류입니다. 성장, 상태, 환경, 자원 중 하나로 구분합니다.")]
    [SerializeField] private StatCategory _category;

    [Header("Range")]
    [Tooltip("스탯이 가질 수 있는 최소값입니다.")]
    [SerializeField] private float _min = 0f;

    [Tooltip("스탯이 가질 수 있는 최대값입니다.")]
    [SerializeField] private float _max = 100f;

    public string Id => _id;
    public string DisplayName => _displayName;
    public StatCategory Category => _category;
    public float Min => _min;
    public float Max => _max;
}