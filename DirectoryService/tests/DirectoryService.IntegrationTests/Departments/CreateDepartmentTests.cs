using DirectoryService.Application.Commands.Departments.CreateDepartment;
using DirectoryService.Domain.Models;
using DirectoryService.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedService.SharedKernel.BaseClasses;

namespace DirectoryService.IntegrationTests.Departments;

public class CreateDepartmentTests : DirectoryTestsBase
{
    private readonly CreateDepartmentHandler _sut;

    public CreateDepartmentTests(DirectoryTestWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<CreateDepartmentHandler>();
    }

    [Fact]
    public async Task CreateParentDepartment_WithValidData_ShouldSucceed()
    {
        // Arrange
        var locations = await Seeder.SeedLocationsAsync(2);

        var command = new CreateDepartmentCommand(
            "It отдел", locations.Select(l => l.Id.Value).ToList());
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _sut.Handle(command, cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();

        var departmentId = result.Value.Id;
        var createdDepartment = await DbContext.Departments
            .Include(d => d.DepartmentLocations)
            .FirstAsync(d => d.Id == Id<Department>.Create(departmentId), cancellationToken: cancellationToken);

        createdDepartment.Should().NotBeNull();
        createdDepartment.Name.Value.Should().Be(command.Name);
        createdDepartment.Path.Should().Be(new LTree("it-otdel"));
        createdDepartment.DepartmentLocations.Should().HaveCount(2);
    }
}