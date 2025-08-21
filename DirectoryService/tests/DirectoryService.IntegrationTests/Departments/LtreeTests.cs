using Dapper;
using DirectoryService.Domain.Models;
using DirectoryService.Domain.ValueObjects.Departments;
using DirectoryService.Domain.ValueObjects.Locations;
using DirectoryService.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SharedService.SharedKernel.BaseClasses;
using Xunit.Abstractions;

namespace DirectoryService.IntegrationTests.Departments;

public class LtreeTests : DirectoryTestsBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public LtreeTests(
        DirectoryTestWebFactory factory,
        ITestOutputHelper testOutputHelper)
        : base(factory)
    {
        _testOutputHelper = testOutputHelper;
        DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    [Fact]
    public async Task GetHierarchy()
    {
        await InitializeHierarchy();

        var recursiveDepartments = await GetHierarchyRecursive("head");

        _testOutputHelper.WriteLine($"Departments count: {recursiveDepartments.Count}");

        // var materializedDepartments = await GetHierarchyLtree("root");

        // recursiveDepartments.Should().BeEquivalentTo(materializedDepartments);
    }

    private async Task<List<DepartmentDto>> GetHierarchyRecursive(string rootPath) // adjacency list
    {
        const string dapperSql = """
                                 WITH RECURSIVE dept_tree AS (
                                     SELECT d.*, 0 AS level
                                     FROM directory_service.departments d
                                     WHERE d.path = @rootPath::ltree
                                     UNION ALL
                                     SELECT c.*, dt.level + 1
                                     FROM directory_service.departments c
                                     JOIN dept_tree dt ON c.parent_id = dt.id)
                                 SELECT id,
                                     parent_id,
                                     name,
                                     path,
                                     depth,
                                     is_active,
                                     created_at,
                                     updated_at,
                                     level
                                 FROM dept_tree
                                 ORDER BY level, id
                                 """;

        var dbConn = DbContext.Database.GetDbConnection();

        var departmentRaws = (await dbConn.QueryAsync<DepartmentDto>(dapperSql, new { rootPath })).ToList();

        var departmentsDict = departmentRaws.ToDictionary(x => x.Id);
        var roots = new List<DepartmentDto>();

        foreach (var row in departmentRaws)
        {
            if (row.ParentId.HasValue &&
                departmentsDict.TryGetValue(row.ParentId.Value, out var parent))
                parent.Children.Add(departmentsDict[row.Id]);
            else
                roots.Add(departmentsDict[row.Id]);
        }

        return roots;
    }

    private async Task InitializeHierarchy()
    {
        var location = Location.Create(
            id: Id<Location>.GenerateNew(),
            name: LocationName.Create("Headquarters").Value,
            address: LocationAddress.Create("City", "Main", "1").Value!,
            timeZone: IANATimeZone.Create("Europe/Moscow").Value
        ).Value!;

        DbContext.Locations.Add(location);
        await DbContext.SaveChangesAsync();

        var rootId = Id<Department>.GenerateNew();
        var engId = Id<Department>.GenerateNew();
        var hrId = Id<Department>.GenerateNew();
        var salesId = Id<Department>.GenerateNew();
        var backendId = Id<Department>.GenerateNew();
        var backendFirstId = Id<Department>.GenerateNew();

        var root = Department.Create(
            id: rootId,
            name: DepartmentName.Create("Head").Value,
            parent: null).Value!;
        root.UpdateLocations([location.Id]);

        var eng = Department.Create(
            id: engId,
            name: DepartmentName.Create("Engineering").Value,
            parent: root).Value!;
        eng.UpdateLocations([location.Id]);
        root.IncreaseChildrenCount();

        var backend = Department.Create(
            id: backendId,
            name: DepartmentName.Create("Backend").Value,
            parent: eng).Value!;
        backend.UpdateLocations([location.Id]);
        eng.IncreaseChildrenCount();

        var backTeam1 = Department.Create(
            id: backendFirstId,
            name: DepartmentName.Create("BackendFirstTeam").Value,
            parent: backend).Value!;
        backTeam1.UpdateLocations([location.Id]);
        backend.IncreaseChildrenCount();

        var hr = Department.Create(
            id: hrId,
            name: DepartmentName.Create("HumanResources").Value,
            parent: root).Value!;
        hr.UpdateLocations([location.Id]);
        root.IncreaseChildrenCount();

        var sales = Department.Create(
            id: salesId,
            name: DepartmentName.Create("Sales").Value,
            parent: root).Value!;
        sales.UpdateLocations([location.Id]);
        root.IncreaseChildrenCount();

        DbContext.Departments.AddRange(root, eng, backend, backTeam1, hr, sales);
        await DbContext.SaveChangesAsync();
    }
}

public sealed class DepartmentDto
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;
    public int Depth { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public List<DepartmentDto> Children { get; set; } = [];
}