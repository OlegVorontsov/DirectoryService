using System.Data;
using DirectoryService.Infrastructure.DataBase.Write;
using Microsoft.EntityFrameworkCore.Storage;
using SharedService.Core.Database.Intefraces;

namespace DirectoryService.Infrastructure.DataBase;

public class DirectoryServiceUnitOfWork(
    ApplicationWriteDBContext context) : IUnitOfWork
{
    public async Task<IDbTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        return transaction.GetDbTransaction();
    }
}