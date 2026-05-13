using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillCardDefinition", menuName = "Scriptable Objects/SkillCardDefinition")]
public class SkillCardDefinition : CardDefinition
{
    [SerializeField] private string _id;
    [SerializeField] private string _displayName;
    [SerializeField] private string _description;

    [SerializeField] private SkillCardRole _role;
    [SerializeField] private NumberRequirementType _numberRequirementType;
    [SerializeField] private int _requiredNumber;
    [SerializeField] private int _requiredCount;
    [SerializeField] private List<CardEffect> _effects;

    public string Id => _id;
    public string DisplayName => _displayName;
    public string Description => _description;
    public int RequiredNumber => _requiredNumber;
    public int RequiredCount => _requiredCount;
    public SkillCardRole Role => _role;
    public NumberRequirementType NumberRequirementType => _numberRequirementType;
    public List<CardEffect> Effects => _effects;
    public override string ToString()
    {
        return $"SkillCardDefinition(Id: {_id}, DisplayName: {_displayName}, Description: {_description}, Role: {_role}, NumberRequirementType: {_numberRequirementType}, RequiredNumber: {_requiredNumber}, RequiredCount: {_requiredCount}, Effects: {string.Join(", ", _effects)})";
    }
}


public enum SkillCardRole
{
    Attack,
    Defense,
    Heal,
    Counter,
    HeavyAttack,
    AreaAttack,
    OrderSupport
}

public enum NumberRequirementType
{
    /// <summary>
    /// 아무 조건 없음
    /// </summary>
    Any,

    /// <summary>
    /// 기준값 이상
    /// </summary>
    GreaterOrEqual,

    /// <summary>
    /// 기준값 이하
    /// </summary>
    LessOrEqual,

    /// <summary>
    /// 기준값과 같음
    /// </summary>
    Equal,

    /// <summary>
    /// 홀수
    /// </summary>
    Odd,

    /// <summary>
    /// 짝수
    /// </summary>
    Even,

    /// <summary>
    /// 범위 내 값
    /// </summary>
    Range,

    /// <summary>
    /// 두 개
    /// </summary>
    Both
}