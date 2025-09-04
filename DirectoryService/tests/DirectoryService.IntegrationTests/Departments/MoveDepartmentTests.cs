using DirectoryService.Application.Commands.Departments.MoveDepartment;
using DirectoryService.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.IntegrationTests.Departments;

public class MoveDepartmentTests : DirectoryTestsBase
{
    private readonly MoveDepartmentHandler _sut;

    public MoveDepartmentTests(DirectoryTestWebFactory factory) : base(factory)
    {
        _sut = Scope.ServiceProvider.GetRequiredService<MoveDepartmentHandler>();
    }

    [Fact]
    public async Task MoveDepartment_UnderAnotherParent_UpdatesPathAndDepth()
    {
        // arrange
        var root = await Seeder.SeedParentDepartmentAsync("It otdel"); // it
        var alpha = await Seeder.SeedChildDepartmentAsync(root, "Alpha"); // it-otdel.alpha
        var beta = await Seeder.SeedChildDepartmentAsync(root, "Beta"); // it-otdel.beta
        var gamma = await Seeder.SeedChildDepartmentAsync(alpha, "Gamma"); // it-otdel.alpha.gamma

        // act
        var command = new MoveDepartmentCommand(alpha.Id.Value, beta.Id.Value); // alpha в beta
        var result = await _sut.Handle(command, CancellationToken.None);

        DbContext.ChangeTracker.Clear();

        // assert
        result.IsSuccess.Should().BeTrue();

        var updatedAlpha = await DbContext.Departments.FirstAsync(d => d.Id == alpha.Id);
        var updatedGamma = await DbContext.Departments.FirstAsync(d => d.Id == gamma.Id);

        updatedAlpha.ParentId.Should().Be(beta.Id);
        updatedAlpha.Path.Should().Be(new LTree("it-otdel.beta.alpha"));
        updatedAlpha.Depth.Should().Be(2);

        updatedGamma.ParentId.Should().Be(updatedAlpha.Id);
        updatedGamma.Path.Should().Be(new LTree("it-otdel.beta.alpha.gamma"));
        updatedGamma.Depth.Should().Be(3);
    }

    [Fact]
    public async Task MoveDepartment_ToRoot_UpdatesPathAndDepth()
    {
        // arrange
        var root = await Seeder.SeedParentDepartmentAsync("It Отдел");
        var a = await Seeder.SeedChildDepartmentAsync(root, "Alpha");
        var x = await Seeder.SeedChildDepartmentAsync(a, "X Ray");

        // act
        var command = new MoveDepartmentCommand(a.Id.Value, null);
        var result = await _sut.Handle(command, CancellationToken.None);

        DbContext.ChangeTracker.Clear();

        // assert
        result.IsSuccess.Should().BeTrue();

        var updatedA = await DbContext.Departments.FirstAsync(d => d.Id == a.Id);
        var updatedX = await DbContext.Departments.FirstAsync(d => d.Id == x.Id);

        updatedA.ParentId.Should().BeNull();
        updatedA.Path.Should().Be(new LTree("alpha"));
        updatedA.Depth.Should().Be(0);

        updatedX.ParentId.Should().Be(updatedA.Id);
        updatedX.Path.Should().Be(new LTree("alpha.x-ray"));
        updatedX.Depth.Should().Be(1);
    }
}