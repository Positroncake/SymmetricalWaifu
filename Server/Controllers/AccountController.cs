using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server.Controllers;

[ApiController]
[Route("AccountApi")]
public class AccountController : ControllerBase
{
    //Todo: Add username and display name length constraints
    
    [HttpPost]
    [Route("Register")]
    public async Task<ActionResult> Register([FromBody] RegistrationRequest registrationRequest)
    {
        IAccess access = new Access();
        // Check if username is taken
        const string checkForUsername = "SELECT * FROM accounts WHERE Username = @Username LIMIT 1";
        List<AccountDbObject> result = await access.QueryAsync<AccountDbObject, dynamic>(checkForUsername, new
        {
            registrationRequest.Username
        }, Utils.ConnectionString);
        if (result.Count is 1) return Conflict("Username already exists");

        // Store new account into database
        const string sql =
            "INSERT INTO accounts (Username, PasswordHash, PasswordSalt, DisplayName, Email, Joined, Submissions, WinningSubmissions) VALUES (@Username, @PasswordHash, @PasswordSalt, @DisplayName, @Email, @Joined, @Submissions, @WinningSubmissions)";
        await access.ExecuteAsync(sql, new
        {
            registrationRequest.Username,
            registrationRequest.PasswordHash,
            registrationRequest.PasswordSalt,
            registrationRequest.DisplayName,
            registrationRequest.Email,
            Joined = DateTime.UtcNow,
            Submissions = 0,
            WinningSubmissions = 0
        }, Utils.ConnectionString);

        string token = await Utils.NewToken(registrationRequest.Username);
        return Ok(token);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        (bool result, AccountDbObject? selected) = await Utils.Login(loginRequest);
        if (!result) return Unauthorized();
        string token = await Utils.NewToken(selected!.Username);
        return Ok(token);
    }
}