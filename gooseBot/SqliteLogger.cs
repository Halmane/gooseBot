using Dapper;
using Microsoft.Data.Sqlite;

namespace gooseBot;

public class SqliteLogger : IDisposable
{
    private SqliteConnection _connection;

    public SqliteLogger(string path)
    {
        _connection = new SqliteConnection($"Data Source={path}");
        _connection.Open();
    }

    public async Task LogIntoDbAsync(string source, string logMessage)
    {
        await _connection.ExecuteAsync(
            "Insert into Log(DateTime,Source,LogMessage) Values(@dateTime,@source,@log);",
            new { source = source, log = logMessage, dateTime = DateTime.UtcNow }
        );
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
