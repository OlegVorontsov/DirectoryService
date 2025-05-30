using System.Text;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Domain.ValueObjects.DepartmentValueObjects;

public class DepartmentPath
{
    private static Dictionary<char, string> _replaceChars = [];
    public LTree Value { get; }

    private DepartmentPath(string value)
    {
        Value = value;
    }

    // EF Core
    private DepartmentPath() { }

    public static DepartmentPath CreateFromExisting(LTree existingPath) =>
        new (existingPath);

    public static DepartmentPath CreateFromStringAndParent(string stringToSlug, string? parentPath)
    {
        // replace all white spaces with -
        stringToSlug = new Regex(@"\s+").Replace(stringToSlug.Trim().ToLower(), "-");

        // replace all characters, specified in settings
        var sb = new StringBuilder();
        foreach (var ch in stringToSlug)
        {
            if (_replaceChars.TryGetValue(ch, out var replacement))
                sb.Append(replacement);
            else
                sb.Append(ch);
        }

        stringToSlug = sb.ToString();

        // remove all characters, except lower case latin, digits, '.' and '-'
        stringToSlug = new Regex(@"[^a-z0-9\-\.]").Replace(stringToSlug, string.Empty);

        return parentPath is null ?
            new DepartmentPath(stringToSlug) :
            new DepartmentPath($"{parentPath}.{stringToSlug}");
    }

    public static UnitResult<Error> SetReplaceChars(Dictionary<char, string> replaceChars)
    {
        if (replaceChars.Count == 0)
            return Error.Failure("application.failure", "Couldn't set _replaceChars again");

        _replaceChars = replaceChars;
        return UnitResult.Success<Error>();
    }
}