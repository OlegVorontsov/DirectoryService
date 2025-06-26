using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Domain.Models;
using DirectoryService.Infrastructure.DataBase.Write;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Infrastructure.DataBase.Repositories;

public class DepartmentLocationRepository(
    ApplicationWriteDBContext context) : IDepartmentLocationRepository
{
    public void Remove(IEnumerable<DepartmentLocation> departmentLocations)
    {
        context.DepartmentLocations.RemoveRange(departmentLocations);
    }
}