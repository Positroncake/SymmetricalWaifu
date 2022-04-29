using System.Security.Cryptography;
using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server.Controllers;

[ApiController]
[Route("TokenApi")]
public class TokenController : ControllerBase
{
    private const String ConnectionString = Access.ConnectionString;
    
    [HttpGet]
    [Route("NewFromLoginDetails")]
    public async Task<ActionResult> NewFromLoginDetails(LoginRequest loginRequest)
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
        if (hash.SequenceEqual(selected.PasswordHash))
        {
            // Generate token
        }
        return Ok();
    }
    
    [HttpGet]
    [Route("NewFromExistingToken")]
    public ActionResult NewFromExistingToken(String existingToken)
    {
        return new StatusCodeResult(StatusCodes.Status501NotImplemented);
    }

    [NonAction]
    private String GenToken()
    {
        Span<Byte> bytes = stackalloc Byte[64];
        RandomNumberGenerator.Fill(bytes);
    }
}