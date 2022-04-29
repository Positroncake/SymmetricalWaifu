using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace DatabaseAccess;

public class Access : IAccess
{
    public const String ConnectionString = "Server=127.0.0.1;Port=3306;Database=symmetrical_waifu;Uid=waifudatabase;Pwd=JUJqzeFfrUozFV5Wpxuxh3mSwXzrsPq7";
    
    public async Task<List<T>> Load<T, TU>(String sql, TU parameters, String connectionString)
    {
        using IDbConnection connection = new MySqlConnection(connectionString);
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        IEnumerable<T> rows = await connection.QueryAsync<T>(sql, parameters);
        return rows.ToList();
    }
    
    public Task Save<T>(String sql, T parameters, String connectionString)
    {
        using IDbConnection connection = new MySqlConnection(connectionString);
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        return connection.ExecuteAsync(sql, parameters);
    }
}