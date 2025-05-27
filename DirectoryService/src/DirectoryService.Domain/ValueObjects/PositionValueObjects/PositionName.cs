using CSharpFunctionalExtensions;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Domain.ValueObjects.PositionValueObjects;

public class PositionName
{
    public const int MAX_LENGTH = 100;
    public const int MIN_LENGTH = 3;

    public string Value { get; private set; }

    public static Result<PositionName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsRequired(nameof(PositionName));

        if (value.Length < MIN_LENGTH || value.Length > MAX_LENGTH)
            return Errors.General.ValueIsInvalid(nameof(PositionName));

        return new PositionName(value);
    }

    private PositionName(string value)
    {
        Value = value;
    }

    // EF Core
    private PositionName() { }
}