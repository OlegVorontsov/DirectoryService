using System.Data;
using DirectoryService.Application.Interfaces.DataBase;
using Microsoft.EntityFrameworkCore.Storage;

namespace DirectoryService.Infrastructure.DataBase;

public class DirectoryServiceUnitOfWork(
    ApplicationDBContext context) : IUnitOfWork
{
    public async Task<IDbTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }
}