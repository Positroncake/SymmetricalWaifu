namespace DatabaseAccess;

public interface IAccess
{
    Task<List<T>> Query<T, TU>(String sql, TU parameters, String connectionString);
    Task Execute<T>(String sql, T parameters, String connectionString);
}