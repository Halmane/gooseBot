using Dapper;
using Microsoft.Data.Sqlite;

namespace gooseBot;

public class SqliteController : IDisposable
{
    private SqliteConnection _connection;

    public SqliteController(string path)
    {
        _connection = new SqliteConnection($"Data Source={path}");
        _connection.Open();
    }

    public void Logger(string source, string logMessage)
    {
        _connection.Execute(
            "Insert into Log(DateTime,Source,LogMessage) Values(@dateTime,@source,@log);",
            new { source = source, log = logMessage, dateTime = DateTime.Now }
        );
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
