using System.Data;
using Dapper;
using DirectoryService.Application.Shared.Database;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Application.Shared.Extensions;

public static class DBConnectionExtensions
{
    /// <summary>
    /// Hides call with CommandDefinition (to propagate CancellationToken) and SQL statement logging.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="builder"></param>
    /// <param name="logger"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<T?> QueryFirstOrDefaultAsync<T>(
        this IDbConnection connection,
        CustomSQLBuilder builder,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        string sql = builder.ToString();
        logger?.LogInformation(sql);

        return await connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(
            sql,
            parameters: builder.Parameters,
            cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Hides call with CommandDefinition (to propagate CancellationToken) and SQL statement logging.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="builder"></param>
    /// <param name="logger"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<T>> QueryAsync<T>(
        this IDbConnection connection,
        CustomSQLBuilder builder,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        string sql = builder.ToString();
        logger?.LogInformation(sql);

        return await connection.QueryAsync<T>(new CommandDefinition(
            sql,
            parameters: builder.Parameters,
            cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Hides call with CommandDefinition (to propagate CancellationToken) and SQL statement logging.
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="builder"></param>
    /// <param name="logger"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<SqlMapper.GridReader> QueryMultipleAsync(
        this IDbConnection connection,
        CustomSQLBuilder builder,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        string sql = builder.ToString();
        logger?.LogInformation(sql);

        return await connection.QueryMultipleAsync(new CommandDefinition(
            sql,
            parameters: builder.Parameters,
            cancellationToken: cancellationToken));
    }

    /// <summary>
    /// Hides call with CommandDefinition (to propagate CancellationToken) and SQL statement logging.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="connection"></param>
    /// <param name="builder"></param>
    /// <param name="logger"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<T?> ExecuteScalarAsync<T>(
        this IDbConnection connection,
        CustomSQLBuilder builder,
        ILogger? logger = null,
        CancellationToken cancellationToken = default)
    {
        string sql = builder.ToString();
        logger?.LogInformation(sql);

        return await connection.ExecuteScalarAsync<T>(new CommandDefinition(
            sql,
            parameters: builder.Parameters,
            cancellationToken: cancellationToken));
    }
}