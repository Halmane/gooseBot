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

    public async Task LogIntoDb(string source, string logMessage)
    {
        _connection.Execute(
            "Insert into Log(DateTime,Source,LogMessage) Values(@dateTime,@source,@log);",
            new { source = source, log = logMessage, dateTime = DateTime.UtcNow }
        );
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
