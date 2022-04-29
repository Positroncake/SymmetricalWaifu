using System.Security.Cryptography;
using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server.Controllers;

[ApiController]
[Route("AccountApi")]
public class AccountController : ControllerBase
{
    private const String ConnectionString = Access.ConnectionString;

    [HttpPost]
    [Route("Register")]
    public async Task<ActionResult> Register([FromBody] RegistrationRequest registrationRequest)
    {
        IAccess access = new Access();
        // Todo: Check if username is taken.
        const String sql =
            "INSERT INTO accounts (Username, PasswordHash, PasswordSalt, DisplayName, Email, Joined, Submissions, WinningSubmissions) VALUES (@Username, @PasswordHash, @PasswordSalt, @DisplayName, @Email, @Joined, @Submissions, @WinningSubmissions)";
        await access.Save(sql, new
        {
            Username = registrationRequest.Username,
            PasswordHash = registrationRequest.PasswordHash,
            PasswordSalt = registrationRequest.PasswordSalt,
            DisplayName = registrationRequest.DisplayName,
            Email = registrationRequest.Email,
            Joined = System.DateTime.UtcNow,
            Submissions = 0,
            WinningSubmissions = 0
        }, ConnectionString);
        return Ok("Done. Please go to page '/' to view the new account!");
    }

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        IAccess access = new Access();
        const String sql = "SELECT * FROM symmetrical_waifu.accounts WHERE Username = @LoginUsername";
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
        if (hash.SequenceEqual(selected.PasswordHash)) return Ok(selected);
        return new UnauthorizedResult();
    }
}