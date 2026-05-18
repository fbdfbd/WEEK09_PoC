using System;
using System.Collections.Generic;

/// <summary>
/// 외부 시스템들이 엔티티의 컴포넌트를 쉽게 조회/생성/수정/삭제할 수 있게 해주는 중앙 상태 저장소
/// </summary>
[Serializable]
public class WorldState
{
    private readonly Dictionary<EntityId, StatComponent> _statComponents = new();
    private readonly Dictionary<EntityId, ControlStateComponent> _controlStateComponents = new();

    public IReadOnlyDictionary<EntityId, StatComponent> StatComponents => _statComponents;
    public IReadOnlyDictionary<EntityId, ControlStateComponent> ControlStateComponents => _controlStateComponents;

    // -------------------------
    // StatComponent
    // -------------------------

    public bool HasStatComponent(EntityId entityId)
    {
        return entityId.IsValid && _statComponents.ContainsKey(entityId);
    }

    public StatComponent GetOrCreateStatComponent(EntityId entityId)
    {
        ValidateEntityId(entityId);

        if (!_statComponents.TryGetValue(entityId, out var component))
        {
            component = new StatComponent();
            _statComponents[entityId] = component;
        }

        return component;
    }

    public bool TryGetStatComponent(EntityId entityId, out StatComponent component)
    {
        if (!entityId.IsValid)
        {
            component = null;
            return false;
        }

        return _statComponents.TryGetValue(entityId, out component);
    }

    public bool RemoveStatComponent(EntityId entityId)
    {
        if (!entityId.IsValid)
            return false;

        return _statComponents.Remove(entityId);
    }

    // -------------------------
    // ControlStateComponent
    // -------------------------

    public bool HasControlStateComponent(EntityId entityId)
    {
        return entityId.IsValid && _controlStateComponents.ContainsKey(entityId);
    }

    public ControlStateComponent GetOrCreateControlStateComponent(EntityId entityId)
    {
        ValidateEntityId(entityId);

        if (!_controlStateComponents.TryGetValue(entityId, out var component))
        {
            component = new ControlStateComponent();
            _controlStateComponents[entityId] = component;
        }

        return component;
    }

    public bool TryGetControlStateComponent(EntityId entityId, out ControlStateComponent component)
    {
        if (!entityId.IsValid)
        {
            component = null;
            return false;
        }

        return _controlStateComponents.TryGetValue(entityId, out component);
    }

    public void SetControlState(EntityId entityId, ActionType actionType)
    {
        var component = GetOrCreateControlStateComponent(entityId);
        component.Set(actionType);
    }

    public ActionType GetControlStateOrDefault(EntityId entityId)
    {
        if (TryGetControlStateComponent(entityId, out var component))
            return component.CurrentAction;

        return ActionType.Allow;
    }

    public bool RemoveControlStateComponent(EntityId entityId)
    {
        if (!entityId.IsValid)
            return false;

        return _controlStateComponents.Remove(entityId);
    }

    // -------------------------
    // Utility
    // -------------------------

    public void Clear()
    {
        _statComponents.Clear();
        _controlStateComponents.Clear();
    }

    private static void ValidateEntityId(EntityId entityId)
    {
        if (!entityId.IsValid)
            throw new ArgumentException("유효하지 않은 EntityId입니다.", nameof(entityId));
    }
}