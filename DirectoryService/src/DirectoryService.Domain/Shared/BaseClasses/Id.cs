namespace DirectoryService.Domain.Shared.BaseClasses;

public class Id<TOwner> : IComparable<Id<TOwner>>
    where TOwner : class
{
    public Guid Value { get; }

    public static Id<TOwner> GenerateNew() => new(Guid.NewGuid());

    public static Id<TOwner> Empty() => new(Guid.Empty);

    public static Id<TOwner> Create(Guid id) => new(id);

    public int CompareTo(Id<TOwner>? other) => Value.CompareTo(other?.Value);

    // EF Core
    protected Id() { }

    protected Id(Guid value)
    {
        Value = value;
    }
}