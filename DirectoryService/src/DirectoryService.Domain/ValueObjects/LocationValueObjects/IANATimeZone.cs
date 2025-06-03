using CSharpFunctionalExtensions;
using NodaTime;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Domain.ValueObjects.LocationValueObjects;

public class IANATimeZone
{
    public string Value { get; private set; }

    public static Result<IANATimeZone, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsRequired(nameof(IANATimeZone));

        if (DateTimeZoneProviders.Tzdb.GetZoneOrNull(value) is null)
            return Errors.General.ValueIsInvalid(nameof(IANATimeZone));

        return new IANATimeZone(value);
    }

    private IANATimeZone(string value)
    {
        Value = value;
    }

    // EF Core
    private IANATimeZone() { }
}