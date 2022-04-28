namespace DatabaseAccess;

public interface IAccess
{
    Task<List<T>> Load<T, TU>(String sql, TU parameters, String connectionString);
    Task Save<T>(String sql, T parameters, String connectionString);
}