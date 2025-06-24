using System.Data;

namespace DirectoryService.Application.Shared.Interfaces;

public interface IDBConnectionFactory
{
    public IDbConnection Create();
}