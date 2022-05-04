using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace DatabaseAccess;

public class Access : IAccess
{
    public async Task<List<T>> Query<T, TU>(String sql, TU parameters, String connectionString)
    {
        using IDbConnection connection = new MySqlConnection(connectionString);
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        IEnumerable<T> rows = await connection.QueryAsync<T>(sql, parameters);
        return rows.ToList();
    }
    
    public Task Execute<T>(String sql, T parameters, String connectionString)
    {
        using IDbConnection connection = new MySqlConnection(connectionString);
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        return connection.ExecuteAsync(sql, parameters);
    }
}