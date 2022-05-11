using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server.Controllers;

[ApiController]
[Route("AccountApi")]
public class AccountController : ControllerBase
{
    [HttpPost]
    [Route("Register")]
    public async Task<ActionResult> Register([FromBody] RegistrationRequest registrationRequest)
    {
        for (var i = 0; i < 5; ++i)
        {
            try
            {
                IAccess access = new Access();
                // Todo: Check if username is taken.
                const String sql =
                    "INSERT INTO accounts (Username, PasswordHash, PasswordSalt, DisplayName, Email, Joined, Submissions, WinningSubmissions) VALUES (@Username, @PasswordHash, @PasswordSalt, @DisplayName, @Email, @Joined, @Submissions, @WinningSubmissions)";
                await access.ExecuteAsync(sql, new
                {
                    Username = registrationRequest.Username,
                    PasswordHash = registrationRequest.PasswordHash,
                    PasswordSalt = registrationRequest.PasswordSalt,
                    DisplayName = registrationRequest.DisplayName,
                    Email = registrationRequest.Email,
                    Joined = System.DateTime.UtcNow,
                    Submissions = 0,
                    WinningSubmissions = 0
                }, Utils.ConnectionString);

                String token = await Utils.NewToken(registrationRequest.Username);
                return Ok(token);
            }
            catch (MySqlException e)
            {
                Console.WriteLine($"{DateTime.UtcNow} | CONN_ERR / RGS");
            }
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    [HttpPost]
    [Route("Login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest loginRequest)
    {
        for (var i = 0; i < 5; ++i)
        {
            try
            {
                (Boolean result, Account? selected) = await Utils.Login(loginRequest);
                if (!result) return Unauthorized();
                String token = await Utils.NewToken(selected!.Username);
                return Ok(token);
            }
            catch (MySqlException e)
            {
                Console.WriteLine($"{DateTime.UtcNow} | CONN_ERR / LGN");
            }
        }

        return new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }
}