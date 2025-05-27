using CSharpFunctionalExtensions;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Domain.ValueObjects.LocationValueObjects;

public class LocationAddress : IComparable<LocationAddress>
{
    public const int CITY_MAX_LENGTH = 100;
    public const int STREET_MAX_LENGTH = 100;
    public const int HOUSENUMBER_MAX_LENGTH = 10;

    public string City { get; private set; }
    public string Street { get; private set; }
    public string HouseNumber { get; private set; }

    // EF Core
    private LocationAddress() { }

    private LocationAddress(
        string city,
        string street,
        string houseNumber)
    {
        City = city;
        Street = street;
        HouseNumber = houseNumber;
    }

    public static Result<LocationAddress, Error> Create(
        string city,
        string street,
        string houseNumber)
    {
        if (string.IsNullOrWhiteSpace(city) ||
            string.IsNullOrWhiteSpace(street) ||
            string.IsNullOrWhiteSpace(houseNumber))
            return Errors.General.ValueIsInvalid(nameof(LocationAddress));

        if (city.Length > CITY_MAX_LENGTH ||
            street.Length > STREET_MAX_LENGTH ||
            houseNumber.Length > HOUSENUMBER_MAX_LENGTH)
            return Errors.General.ValueIsInvalid(nameof(LocationAddress));

        return new LocationAddress(city, street, houseNumber);
    }

    public int CompareTo(LocationAddress? other)
    {
        if (other is null) throw new ArgumentNullException(nameof(other));

        return City == other.City && Street == other.Street && HouseNumber == other.HouseNumber ? 0 : -1;
    }
}