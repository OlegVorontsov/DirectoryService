using CSharpFunctionalExtensions;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Domain.ValueObjects.PositionValueObjects;

public class PositionDescription
{
    public const int MAX_LENGTH = 1000;

    public string Value { get; private set; }

    public static Result<PositionDescription, Error> Create(string value)
    {
        if (value.Length > MAX_LENGTH)
            return Errors.General.ValueIsInvalid(nameof(PositionDescription));

        return new PositionDescription(value);
    }

    private PositionDescription(string value)
    {
        Value = value;
    }

    // EF Core
    private PositionDescription() { }
}