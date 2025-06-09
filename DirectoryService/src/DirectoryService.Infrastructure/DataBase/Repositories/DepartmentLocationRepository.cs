using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Domain.Models;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Infrastructure.DataBase.Repositories;

public class DepartmentLocationRepository(
    ApplicationDBContext context) : IDepartmentLocationRepository
{
    public void Remove(IEnumerable<DepartmentLocation> departmentLocations)
    {
        context.DepartmentLocations.RemoveRange(departmentLocations);
    }
}