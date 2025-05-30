using System.Data;

namespace DirectoryService.Application.Interfaces.DataBase;

public interface IUnitOfWork
{
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}