using CSharpFunctionalExtensions;
using DirectoryService.Domain.Shared.BaseClasses;
using DirectoryService.Domain.ValueObjects.PositionValueObjects;

namespace DirectoryService.Domain.Models;

public class Position
{
    public Id<Position> Id { get; private set; }
    public PositionName Name { get; private set; }
    public PositionDescription Description { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public static Result<Position> Create(
        Id<Position> id,
        PositionName name,
        PositionDescription description,
        DateTime createdAt)
    {
        return new Position(
            id,
            name,
            description,
            createdAt);
    }

    // EF Core
    private Position() { }

    private Position(
        Id<Position> id,
        PositionName name,
        PositionDescription description,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = CreatedAt;
    }
}