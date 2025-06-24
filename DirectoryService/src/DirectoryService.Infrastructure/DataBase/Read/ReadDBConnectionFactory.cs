using System.Data;
using DirectoryService.Application.Shared.Interfaces;
using Npgsql;

namespace DirectoryService.Infrastructure.DataBase.Read;

public class ReadDBConnectionFactory : IDBConnectionFactory
{
    private readonly string _connectionString;

    public ReadDBConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Create()
    {
        return new NpgsqlConnection(_connectionString);
    }
}