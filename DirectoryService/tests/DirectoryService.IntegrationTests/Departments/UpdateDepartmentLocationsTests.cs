using DirectoryService.Application.Commands.Departments.UpdateDepartment;
using DirectoryService.Contracts.Requests;
using DirectoryService.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class UpdateDepartmentLocationsTests : DirectoryTestsBase
{
    private readonly UpdateDepartmentHandler _sut;

    public UpdateDepartmentLocationsTests(DirectoryTestWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<UpdateDepartmentHandler>();
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldUpdateDepartmentLocations()
    {
        // Arrange
        var department = await Seeder.SeedParentDepartmentAsync("Test Department");
        var newLocation = await Seeder.SeedLocationAsync("New Location");

        var request = new UpdateDepartmentRequest(
            "Test Department",
            [newLocation.Id.Value]);

        var command = request.ToCommand(department.Id.Value);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var updatedDepartment = await DbContext.Departments
            .Include(d => d.DepartmentLocations)
            .FirstAsync(d => d.Id == department.Id);

        updatedDepartment.DepartmentLocations.Should().HaveCount(1);
        updatedDepartment.DepartmentLocations.Should().ContainSingle(dl => dl.LocationId == newLocation.Id);
    }

    [Fact]
    public async Task Handle_WithNonExistentDepartment_ShouldReturnNotFound()
    {
        // Arrange
        var location = await Seeder.SeedLocationAsync();

        var request = new UpdateDepartmentRequest(
            "Test Department",
            [location.Id.Value]);

        var command = request.ToCommand(Guid.NewGuid());

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithNonExistentLocation_ShouldReturnNotFound()
    {
        // Arrange
        var department = await Seeder.SeedParentDepartmentWithoutLocationAsync("Test Department");

        var request = new UpdateDepartmentRequest(
            "Test Department",
            [Guid.NewGuid()]);

        var command = request.ToCommand(department.Id.Value);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().ContainSingle(e => e.Code == " Locations ");
    }

    [Fact]
    public async Task Handle_WithEmptyLocationList_ShouldReturnValidationError()
    {
        // Arrange
        var department = await Seeder.SeedParentDepartmentAsync("Test Department");

        var request = new UpdateDepartmentRequest(
            "Test Department",
            []);

        var command = request.ToCommand(department.Id.Value);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().ContainSingle(e => e.Message == "invalid  LocationIds  lenght");
    }
}