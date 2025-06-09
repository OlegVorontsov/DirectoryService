namespace DirectoryService.Domain.Shared.BaseClasses;

public class Id<TOwner> : IComparable<Id<TOwner>>, IEquatable<Id<TOwner>>
    where TOwner : class
{
    public Guid Value { get; private set; }

    public static Id<TOwner> GenerateNew() => new(Guid.NewGuid());

    public static Id<TOwner> Empty() => new(Guid.Empty);

    public static Id<TOwner> Create(Guid id) => new(id);

    public int CompareTo(Id<TOwner>? other) => Value.CompareTo(other?.Value);

    public bool Equals(Id<TOwner>? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Id<TOwner> other) return Equals(other);
        return false;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(Id<TOwner>? left, Id<TOwner>? right)
        => Equals(left, right);

    public static bool operator !=(Id<TOwner>? left, Id<TOwner>? right)
        => !Equals(left, right);

    // EF Core
    protected Id() { Value = Guid.Empty; }

    protected Id(Guid value)
    {
        Value = value;
    }
}