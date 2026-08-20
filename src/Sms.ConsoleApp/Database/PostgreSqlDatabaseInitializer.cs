using Microsoft.Extensions.Logging;
using Npgsql;

namespace Sms.ConsoleApp.Database;

public sealed class PostgreSqlDatabaseInitializer(
    string connectionString,
    ILogger<PostgreSqlDatabaseInitializer> logger)
{
    public async Task EnsureExistsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var targetConnection = new NpgsqlConnection(connectionString);
            await targetConnection.OpenAsync(cancellationToken);
            return;
        }
        catch (PostgresException exception)
            when (exception.SqlState == PostgresErrorCodes.InvalidCatalogName)
        {
            // PostgreSQL доступен, но запрашиваемая база данных ещё не существует.
        }

        var target = new NpgsqlConnectionStringBuilder(connectionString);
        if (string.IsNullOrWhiteSpace(target.Database))
        {
            throw new InvalidOperationException("The PostgreSQL connection string must specify a database.");
        }

        var databaseName = target.Database;
        var admin = new NpgsqlConnectionStringBuilder(connectionString)
        {
            Database = "postgres",
            Pooling = false
        };

        await using var adminConnection = new NpgsqlConnection(admin.ConnectionString);
        await adminConnection.OpenAsync(cancellationToken);

        using var commandBuilder = new NpgsqlCommandBuilder();
        var quotedDatabaseName = commandBuilder.QuoteIdentifier(databaseName);
        await using var command = new NpgsqlCommand(
            $"CREATE DATABASE {quotedDatabaseName}",
            adminConnection);

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
            logger.LogInformation("Created PostgreSQL database {DatabaseName}", databaseName);
        }
        catch (PostgresException exception)
            when (exception.SqlState == PostgresErrorCodes.DuplicateDatabase)
        {
            logger.LogInformation(
                "PostgreSQL database {DatabaseName} was created by another application instance",
                databaseName);
        }
    }

}
