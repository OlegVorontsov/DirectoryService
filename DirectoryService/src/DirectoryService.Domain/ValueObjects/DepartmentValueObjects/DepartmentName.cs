using CSharpFunctionalExtensions;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Domain.ValueObjects.DepartmentValueObjects;

public class DepartmentName
{
    public const int MAX_LENGTH = 150;
    public const int MIN_LENGTH = 3;

    public string Value { get; }

    public static Result<DepartmentName, Error> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Errors.General.ValueIsRequired(nameof(DepartmentName));

        if (value.Length < MIN_LENGTH || value.Length > MAX_LENGTH)
            return Errors.General.ValueIsInvalid(nameof(DepartmentName));

        return new DepartmentName(value);
    }

    private DepartmentName(string value)
    {
        Value = value;
    }

    // EF Core
    private DepartmentName() { }
}