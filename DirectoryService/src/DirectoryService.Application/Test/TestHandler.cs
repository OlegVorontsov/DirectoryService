using CSharpFunctionalExtensions;
using FluentValidation;
using SharedService.Core.Abstractions;
using SharedService.Core.Validation;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Test;

public class TestHandler(
    IValidator<TestCommand> validator) :
    ICommandHandler<string, TestCommand>
{
    public async Task<Result<string, ErrorList>> Handle(
        TestCommand command,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
        {
            throw new ApplicationException("Validation Failed");
            return validationResult.ToList();
        }

        var testClass = new TestClass();

        var unitResult = testClass.UnitResultMethod();
        if (unitResult.IsFailure)
            return unitResult.Error;

        var result = testClass.ResultMethod();
        if (result.IsFailure)
            return result.Error.ToErrors();

        return "10";
    }
}

public class TestClass
{
    public UnitResult<string> UnitResultMethod()
    {
        return Result.Success("ok");
    }

    public Result<string, Error> ResultMethod()
    {
        return Errors.General.Failure("fail");
    }
}