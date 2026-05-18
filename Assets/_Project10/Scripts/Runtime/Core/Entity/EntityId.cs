using System;

[Serializable]
public readonly struct EntityId : IEquatable<EntityId>
{
    private readonly string _value;

    public string Value
    {
        get
        {
            if (!IsValid)
                throw new InvalidOperationException("Invalid EntityId는 Value를 가질 수 없습니다.");

            return _value;
        }
    }

    public bool IsValid => !string.IsNullOrWhiteSpace(_value);

    public static EntityId Invalid => default;

    public EntityId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("EntityId는 비어 있을 수 없습니다.", nameof(value));

        _value = value;
    }

    public static EntityId Create(EntityKind kind, string definitionId)
    {
        if (string.IsNullOrWhiteSpace(definitionId))
            throw new ArgumentException("Definition ID는 비어 있을 수 없습니다.", nameof(definitionId));

        return new EntityId($"{kind.ToString().ToLowerInvariant()}:{definitionId}");
    }

    public bool Equals(EntityId other)
    {
        return _value == other._value;
    }

    public override bool Equals(object obj)
    {
        return obj is EntityId other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _value != null ? _value.GetHashCode() : 0;
    }

    public override string ToString()
    {
        return IsValid ? _value : "<Invalid EntityId>";
    }

    public static bool operator ==(EntityId left, EntityId right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(EntityId left, EntityId right)
    {
        return !left.Equals(right);
    }
}