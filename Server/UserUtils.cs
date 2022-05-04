using System.Security.Cryptography;
using DatabaseAccess;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server;

public static class UserUtils
{
    public const String ConnectionString = "Server=127.0.0.1;Port=3306;Database=symmetrical_waifu;Uid=waifudatabase;Pwd=JUJqzeFfrUozFV5Wpxuxh3mSwXzrsPq7";

    public static async Task<(Boolean, Account?)> Login(LoginRequest loginRequest)
    {
        // Get account from database if exists
        IAccess access = new Access();
        const String sql = "SELECT * FROM accounts WHERE Username = @LoginUsername LIMIT 1";
        List<Account> accounts = await access.Query<Account, dynamic>(sql, new
        {
            LoginUsername = loginRequest.Username
        }, ConnectionString);
        
        // Check if account exists
        Account selected;
        if (accounts.Count == 1) selected = accounts.First();
        else return (false, null);

        // Add salt to inputted password
        Byte[] hash = loginRequest.Password.Concat(selected.PasswordSalt).ToArray();
        
        // Hash inputted password
        var sha512 = SHA512.Create();
        for (var i = 0; i < 5000; ++i) hash = sha512.ComputeHash(hash);
        
        // Validation - compare inputted hash against hash in database
        return hash.SequenceEqual(selected.PasswordHash) ? (true, selected) : (false, null);
    }

    public static async Task<String> NewToken(String username)
    {
        IAccess access = new Access();
        String token = GenToken();
        const String query = "INSERT INTO tokens (Token, Username) VALUES (@Token, @Username)";
        await access.Execute(query, new
        {
            Token = token,
            Username = username
        }, ConnectionString);
        return token;
    }

    public static async Task<(Boolean, String)> GetUnameFromToken(String token)
    {
        // Get token from database if exists
        IAccess access = new Access();
        const String sql = "SELECT * FROM tokens WHERE Token = @Token LIMIT 1";
        List<TokenClass> results = await access.Query<TokenClass, dynamic>(sql, new
        {
            Token = token
        }, ConnectionString);
        
        // Check if token exists
        return results.Count == 1 ? (true, results.First().Username) : (false, String.Empty);
    }

    public static async Task<(Boolean, String)> CreateOrGetDirFromUname(String username)
    {
        IAccess access = new Access();
        const String sql = "SELECT * FROM directories WHERE Username = @Username LIMIT 1";
        List<DirectoryClass> directories = await access.Query<DirectoryClass, dynamic>(sql, new
        {
            Username = username
        }, ConnectionString);
        
        // If user exists, return
        if (directories.Count is 1) return (false, directories.First().Dir);
        
        // If user does not exist, create and return
        String directory = GenDir();
        const String add = "INSERT INTO directories (Username, Dir) VALUES (@Username, @Dir)";
        await access.Execute(add, new
        {
            Username = username,
            Dir = directory
        }, ConnectionString);
        
        // Return new directory to user
        return (true, directory);
    }

    private static String GenToken()
    {
        Span<Byte> bytes = stackalloc Byte[64];
        RandomNumberGenerator.Fill(bytes);
        String result = Convert.ToHexString(bytes);
        return result.ToLowerInvariant();
    }

    private static String GenDir()
    {
        Span<Byte> bytes = stackalloc Byte[40];
        RandomNumberGenerator.Fill(bytes);
        String result = Convert.ToHexString(bytes);
        return result.ToLowerInvariant();
    }
}