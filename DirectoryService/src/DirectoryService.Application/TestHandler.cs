using SharedService.Core;
using SharedService.Framework;
using SharedService.SharedKernel;

namespace DirectoryService.Application;

public class TestHandler
{
    private var test = new TestCore();
    private var testFramework = new TestFramework();
    private var testTestSharedKernel = new TestSharedKernel();
}