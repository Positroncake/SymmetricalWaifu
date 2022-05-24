using System.Security.Cryptography;
using DatabaseAccess;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server;

public static class Utils
{
    public const string ConnectionString = "Server=127.0.0.1;Port=3306;Database=symmetrical_waifu;Uid=waifudatabase;Pwd=SEO5tImFglekXDrDbzeTO4VpN7ki7jFX";

    public static async Task<(bool, Account?)> Login(LoginRequest loginRequest)
    {
        // Get account from database if exists
        IAccess access = new Access();
        const string sql = "SELECT * FROM accounts WHERE Username = @LoginUsername LIMIT 1";
        List<Account> accounts = await access.QueryAsync<Account, dynamic>(sql, new
        {
            LoginUsername = loginRequest.Username
        }, ConnectionString);
        
        // Check if account exists
        Account selected;
        if (accounts.Count == 1) selected = accounts.First();
        else return (false, null);

        // Add salt to inputted password
        byte[] hash = loginRequest.Password.Concat(selected.PasswordSalt).ToArray();
        
        // Hash inputted password
        var sha512 = SHA512.Create();
        for (var i = 0; i < 5000; ++i) hash = sha512.ComputeHash(hash);
        
        // Validation - compare inputted hash against hash in database
        return hash.SequenceEqual(selected.PasswordHash) ? (true, selected) : (false, null);
    }

    public static async Task<string> NewToken(string username)
    {
        IAccess access = new Access();
        string token = GenToken();
        const string query = "INSERT INTO tokens (Token, Username) VALUES (@Token, @Username)";
        await access.ExecuteAsync(query, new
        {
            Token = token,
            Username = username
        }, ConnectionString);
        return token;
    }

    public static async Task<(bool, string)> GetUnameFromToken(string token)
    {
        // Get token from database if exists
        IAccess access = new Access();
        const string sql = "SELECT * FROM tokens WHERE Token = @Token LIMIT 1";
        List<TokenClass> results = await access.QueryAsync<TokenClass, dynamic>(sql, new
        {
            Token = token
        }, ConnectionString);
        
        // Check if token exists
        return results.Count == 1 ? (true, results.First().Username) : (false, string.Empty);
    }

    public static async Task<(bool, string)> CreateOrGetDirFromUname(string username)
    {
        IAccess access = new Access();
        const string sql = "SELECT * FROM directories WHERE Username = @Username LIMIT 1";
        List<DirectoryClass> directories = await access.QueryAsync<DirectoryClass, dynamic>(sql, new
        {
            Username = username
        }, ConnectionString);
        
        // If user exists, return
        if (directories.Count is 1) return (false, directories.First().Dir);
        
        // If user does not exist, create and return
        string directory = GenDir();
        const string add = "INSERT INTO directories (Username, Dir) VALUES (@Username, @Dir)";
        await access.ExecuteAsync(add, new
        {
            Username = username,
            Dir = directory
        }, ConnectionString);
        
        // Return new directory to user
        return (true, directory);
    }

    private static string GenToken()
    {
        Span<byte> bytes = stackalloc byte[64];
        RandomNumberGenerator.Fill(bytes);
        string result = Convert.ToHexString(bytes);
        return result.ToLowerInvariant();
    }

    private static string GenDir()
    {
        Span<byte> bytes = stackalloc byte[40];
        RandomNumberGenerator.Fill(bytes);
        string result = Convert.ToHexString(bytes);
        return result.ToLowerInvariant();
    }

    public static string GenId() => Guid.NewGuid().ToString("N");
}