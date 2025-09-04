using CSharpFunctionalExtensions;
using DirectoryService.Application.Interfaces.Repositories;
using DirectoryService.Application.Shared.DTOs;
using DirectoryService.Domain.Models;
using FluentValidation;
using Microsoft.Extensions.Logging;
using SharedService.Core.Abstractions;
using SharedService.Core.Database.Intefraces;
using SharedService.Core.Validation;
using SharedService.SharedKernel.BaseClasses;
using SharedService.SharedKernel.Errors;

namespace DirectoryService.Application.Commands.Departments.MoveDepartment;

public class MoveDepartmentHandler(
    IValidator<MoveDepartmentCommand> validator,
    ILogger<MoveDepartmentHandler> logger,
    IDepartmentRepository departmentRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<DepartmentDTO, MoveDepartmentCommand>
{
    public async Task<Result<DepartmentDTO, ErrorList>> Handle(
        MoveDepartmentCommand command,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(
            command, cancellationToken);
        if (validationResult.IsValid == false)
            return validationResult.ToList();

        var transaction = await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var departmentResult = await departmentRepository.GetByIdWithLock(
                Id<Department>.Create(command.DepartmentId), cancellationToken);
            if (departmentResult.IsFailure)
                return departmentResult.Error.ToErrors();

            Department? newParent = null;
            if (command.NewParentDepartmentId != null)
            {
                var parentResult = await departmentRepository.GetByIdWithLock(
                    Id<Department>.Create(command.NewParentDepartmentId.Value), cancellationToken);
                if (parentResult.IsFailure)
                    return parentResult.Error.ToErrors();

                newParent = parentResult.Value;
            }

            if (newParent != null)
            {
                if (departmentResult.Value.Id == newParent.Id)
                    return Error.Validation("department.move.invalid", "Нельзя переместить подразделение само в себя").ToErrors();

                if (newParent.Path.ToString().StartsWith(departmentResult.Value.Path + ".") ||
                    newParent.Path == departmentResult.Value.Path)
                    return Error.Validation("department.move.invalid", "Нельзя переместить подразделение в своего потомка").ToErrors();
            }

            var department = departmentResult.Value;
            var oldPath = department.Path;

            var depthDeltaResult = department.MoveTo(newParent);
            if (depthDeltaResult.IsFailure) return depthDeltaResult.Error.ToErrors();

            newParent?.IncreaseChildrenCount();

            var lockDescendants = await departmentRepository.LockDescendants(oldPath, cancellationToken);
            if (lockDescendants.IsFailure)
                return lockDescendants.Error.ToErrors();

            var saveChanges = await departmentRepository.SaveChanges(cancellationToken);
            if (saveChanges.IsFailure)
                return saveChanges.Error.ToErrors();

            var saveResult = await departmentRepository.BulkUpdateDescendantsPathAndDepth(
                oldPath,
                department.Path,
                depthDeltaResult.Value,
                DateTime.UtcNow,
                cancellationToken);

            if (saveResult.IsFailure) return saveResult.Error.ToErrors();

            transaction.Commit();
            return DepartmentDTO.FromDomainEntity(departmentResult.Value);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}