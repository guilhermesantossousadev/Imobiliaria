using Microsoft.Data.Sqlite;

namespace Core.Dados;

/// <summary>Fábrica de conexões com o banco SQLite da aplicação.</summary>
public class ConexaoSqlite
{
    private readonly string _connectionString;

    public ConexaoSqlite(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SqliteConnection Criar()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open();
        return connection;
    }
}
