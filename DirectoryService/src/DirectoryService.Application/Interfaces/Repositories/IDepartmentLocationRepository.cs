using CSharpFunctionalExtensions;
using DirectoryService.Domain.Models;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Interfaces.Repositories;

public interface IDepartmentLocationRepository
{
    void Remove(IEnumerable<DepartmentLocation> departmentLocations);
}