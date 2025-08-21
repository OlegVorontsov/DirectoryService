using DirectoryService.Domain.Models;
using DirectoryService.Domain.ValueObjects.Departments;
using DirectoryService.Domain.ValueObjects.Locations;
using DirectoryService.Domain.ValueObjects.Positions;
using DirectoryService.Infrastructure.DataBase.Write;
using SharedService.SharedKernel.BaseClasses;

namespace DirectoryService.IntegrationTests.Seeding;

public class TestDataSeeder(ApplicationWriteDBContext context)
{
    public async Task<Location> SeedLocationAsync(string name = "Test Location")
    {
        var location = Location.Create(
            id: Id<Location>.GenerateNew(),
            name: LocationName.Create(name).Value,
            address: LocationAddress.Create("Test City", "Test Street", "1").Value!,
            timeZone: IANATimeZone.Create("Europe/Moscow").Value
            ).Value!;

        context.Locations.Add(location);
        await context.SaveChangesAsync();

        return location;
    }

    public async Task<List<Location>> SeedLocationsAsync(int count = 3)
    {
        var locations = new List<Location>();

        for (int i = 1; i <= count; i++)
        {
            var location = await SeedLocationAsync($"Test Location {i}");
            locations.Add(location);
        }

        return locations;
    }

    public async Task<Position> SeedPositionAsync(
        string name = "Test Position",
        string? description = null)
    {
        var position = Position.Create(
            id: Id<Position>.GenerateNew(),
            name: PositionName.Create(name).Value!,
            description: PositionDescription
                .Create(description ?? $"Description for {name}").Value!
            ).Value!;

        context.Positions.Add(position);
        await context.SaveChangesAsync();

        return position;
    }

    public async Task<List<Position>> SeedPositionsAsync(int count = 3)
    {
        var positions = new List<Position>();

        for (int i = 1; i <= count; i++)
        {
            var position = await SeedPositionAsync($"Test Position {i}");
            positions.Add(position);
        }

        return positions;
    }

    public async Task<Department> SeedParentDepartmentAsync(
        string name = "Test ParentDepartment")
    {
        var location = await SeedLocationAsync();

        var parentDepartment = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create(name).Value,
            parent: null).Value!;

        parentDepartment.UpdateLocations([location.Id]);

        context.Departments.Add(parentDepartment);
        await context.SaveChangesAsync();

        return parentDepartment;
    }

    public async Task<Department> SeedChildDepartmentAsync(
        Department parent,
        string name = "Test ChildDepartment")
    {
        var location = await SeedLocationAsync();

        var childDepartment = Department.Create(
            id: Id<Department>.GenerateNew(),
            name: DepartmentName.Create(name).Value,
            parent: parent).Value!;

        childDepartment.UpdateLocations([location.Id]);
        parent.IncreaseChildrenCount();

        context.Departments.Add(childDepartment);
        await context.SaveChangesAsync();

        return childDepartment;
    }

    public async Task<List<Department>> SeedDepartmentHierarchyAsync()
    {
        var parent = await SeedParentDepartmentAsync("ParentDepartment");
        var child1 = await SeedChildDepartmentAsync(parent, "ChildDepartment1");
        var child2 = await SeedChildDepartmentAsync(parent, "ChildDepartment2");

        return [parent, child1, child2];
    }

    public async Task<DepartmentPosition> SeedDepartmentPositionAsync(
        Department? department = null,
        Position? position = null)
    {
        department ??= await SeedParentDepartmentAsync();
        position ??= await SeedPositionAsync();

        var departmentPosition = new DepartmentPosition(department.Id, position.Id);

        context.DepartmentPositions.Add(departmentPosition);
        await context.SaveChangesAsync();

        return departmentPosition;
    }

    public async Task<List<DepartmentPosition>> SeedDepartmentPositionsAsync(int count = 3)
    {
        var departmentPositions = new List<DepartmentPosition>();

        for (int i = 1; i <= count; i++)
        {
            var departmentPosition = await SeedDepartmentPositionAsync();
            departmentPositions.Add(departmentPosition);
        }

        return departmentPositions;
    }

    public async Task<TestData> SeedCompleteTestDataAsync()
    {
        var locations = await SeedLocationsAsync(3);
        var positions = await SeedPositionsAsync(5);
        var departments = await SeedDepartmentHierarchyAsync();
        var departmentPositions = await SeedDepartmentPositionsAsync(3);

        return new TestData
        {
            Locations = locations,
            Positions = positions,
            Departments = departments,
            DepartmentPositions = departmentPositions
        };
    }

    public async Task<TestData> SeedMinimalTestDataAsync()
    {
        var location = await SeedLocationAsync();
        var position = await SeedPositionAsync();
        var department = await SeedParentDepartmentAsync();
        var departmentPosition = await SeedDepartmentPositionAsync(department, position);

        return new TestData
        {
            Locations = [location],
            Positions = [position],
            Departments = [department],
            DepartmentPositions = [departmentPosition]
        };
    }
}

public class TestData
{
    public List<Location> Locations { get; set; } = [];
    public List<Position> Positions { get; set; } = [];
    public List<Department> Departments { get; set; } = [];
    public List<DepartmentPosition> DepartmentPositions { get; set; } = [];
}