namespace DatabaseAccess;

public interface IAccess
{
    Task<List<T>> QueryAsync<T, TU>(String sql, TU parameters, String connectionString);
    Task ExecuteAsync<T>(String sql, T parameters, String connectionString);
}