using CSharpFunctionalExtensions;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Domain.ValueObjects.Locations;

public class LocationName
{
    public const int MAX_LENGTH = 120;
    public const int MIN_LENGTH = 3;

    public string Value { get; private set; }

    public static Result<LocationName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsRequired(nameof(LocationName));

        if (value.Length < MIN_LENGTH || value.Length > MAX_LENGTH)
            return Errors.General.ValueIsInvalid(nameof(LocationName));

        return new LocationName(value);
    }

    public static explicit operator string(LocationName locationName) =>
        locationName.Value;

    private LocationName(string value)
    {
        Value = value;
    }

    // EF Core
    private LocationName() { }
}