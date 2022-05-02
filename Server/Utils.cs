using System.Security.Cryptography;
using DatabaseAccess;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server;

public static class Utils
{
    public const String ConnectionString = "Server=127.0.0.1;Port=3306;Database=symmetrical_waifu;Uid=waifudatabase;Pwd=JUJqzeFfrUozFV5Wpxuxh3mSwXzrsPq7";

    public static async Task<(Boolean, Account?)> Login(LoginRequest loginRequest)
    {
        // Get account from database if exists
        IAccess access = new Access();
        const String sql = "SELECT * FROM accounts WHERE Username = @LoginUsername LIMIT 1";
        List<Account> accounts = await access.Load<Account, dynamic>(sql, new
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
        await access.Save(query, new
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
        List<TokenClass> results = await access.Load<TokenClass, dynamic>(sql, new
        {
            Token = token
        }, ConnectionString);
        
        // Check if token exists
        return results.Count == 1 ? (true, results.First().Username) : (false, String.Empty);
    }

    private static String GenToken()
    {
        Span<Byte> bytes = stackalloc Byte[64];
        RandomNumberGenerator.Fill(bytes);
        String result = Convert.ToHexString(bytes);
        return result;
    }
}