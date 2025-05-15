using CSharpFunctionalExtensions;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application;

public class TestHandler
{
    public async Task<Result<string, ErrorList>> Handle(
        CancellationToken cancellationToken)
    {
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