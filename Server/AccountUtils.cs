using System.Security.Cryptography;
using DatabaseAccess;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server;

public static class AccountUtils
{
    public const String ConnectionString = "Server=127.0.0.1;Port=3306;Database=symmetrical_waifu;Uid=waifudatabase;Pwd=JUJqzeFfrUozFV5Wpxuxh3mSwXzrsPq7";

    public static async Task<(Boolean, Account?)> Login(LoginRequest loginRequest)
    {
        IAccess access = new Access();
        const String sql = "SELECT * FROM accounts WHERE Username = @LoginUsername LIMIT 1";
        List<Account> accounts = await access.Load<Account, dynamic>(sql, new
        {
            LoginUsername = loginRequest.Username
        }, ConnectionString);
        Account selected = accounts.First();
        
        // Add salt to password
        Byte[] hash = loginRequest.Password.Concat(selected.PasswordSalt).ToArray();
        
        // Hash
        var sha512 = SHA512.Create();
        for (var i = 0; i < 5000; ++i) hash = sha512.ComputeHash(hash);
        
        // Validation
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

    private static String GenToken()
    {
        Span<Byte> bytes = stackalloc Byte[64];
        RandomNumberGenerator.Fill(bytes);
        String result = Convert.ToHexString(bytes);
        return result;
    }
}