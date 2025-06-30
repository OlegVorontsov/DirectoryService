using System.Reflection;
using System.Text.Json;
using DirectoryService.Domain.Models;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SharedService.Core.Abstractions;

namespace DirectoryService.Application;

public static class ApplicationDependencyInjection
{
    private static readonly Assembly _assembly = Assembly.GetExecutingAssembly();

    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        SlugReplaceDictionary();

        services.AddHandlers(_assembly)
                .AddValidatorsFromAssembly(_assembly);

        return services;
    }

    private static void SlugReplaceDictionary()
    {
        var slugReplaceDictionaryJson = File.ReadAllText("etc/slugReplaceDictionary.json");
        var slugReplaceDictionary = JsonSerializer.Deserialize<Dictionary<char, string>>(slugReplaceDictionaryJson);
        if (slugReplaceDictionary is null)
            throw new ApplicationException("Couldn't deserialize slug settings from slugReplaceDictionary.json");

        var setReplaceCharsResult = Department.SetReplaceChars(slugReplaceDictionary);
        if(setReplaceCharsResult.IsFailure)
            throw new ApplicationException(setReplaceCharsResult.Error.ToString());
    }
}