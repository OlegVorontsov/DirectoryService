using CSharpFunctionalExtensions;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Domain.ValueObjects.LocationValueObjects;

public class IANATimeZone
{
    // longest IANA timezone code is America/North_Dakota/New_Salem - 30 symbols long
    public const int MAX_LENGTH = 30;
    public const int MIN_LENGTH = 3;

    public string Value { get; }

    public static Result<IANATimeZone, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsRequired(nameof(IANATimeZone));

        if (value.Length < MIN_LENGTH || value.Length > MAX_LENGTH)
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