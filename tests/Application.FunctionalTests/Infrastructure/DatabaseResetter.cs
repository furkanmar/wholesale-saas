using Npgsql;
using Respawn;

namespace Wholesale.Application.FunctionalTests.Infrastructure;

internal sealed class DatabaseResetter : IAsyncDisposable
{
    private readonly string _connectionString;
    private Respawner? _respawner;

    private DatabaseResetter(string connectionString) => _connectionString = connectionString;

    public static async Task<DatabaseResetter> CreateAsync(string connectionString)
    {
        var resetter = new DatabaseResetter(connectionString);
        await resetter.InitialiseAsync();
        return resetter;
    }

    private async Task InitialiseAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        _respawner = await Respawner.CreateAsync(connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public"],
        });
    }

    public async Task ResetAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await _respawner!.ResetAsync(connection);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
