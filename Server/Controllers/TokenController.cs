using System.Security.Cryptography;
using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server.Controllers;

[ApiController]
[Route("TokenApi")]
public class TokenController : ControllerBase
{
    [HttpPost]
    [Route("NewFromLoginDetails")]
    public async Task<ActionResult> NewFromLoginDetails(LoginRequest loginRequest)
    {
        (Boolean result, Account? selected) = await AccountUtils.Login(loginRequest);
        if (!result) return Unauthorized();
        String token = await AccountUtils.NewToken(selected!.Username);
        return Ok(token);
    }
    
    [HttpPost]
    [Route("NewFromExistingToken")]
    public async Task<ActionResult> NewFromExistingToken(TokenClass existingToken)
    {
        // Validate existing token
        IAccess access = new Access();
        const String sql = "SELECT * FROM tokens WHERE Token = @Token LIMIT 1";
        List<TokenClass> results = await access.Load<TokenClass, dynamic>(sql, new
        {
            Token = existingToken.Token
        }, AccountUtils.ConnectionString);
        
        // Validation
        if (results.Count == 0) return Unauthorized();
        
        // Generate new token
        String token = await AccountUtils.NewToken(results.First().Username);
        return Ok(token);
    }

    [HttpPost]
    [Route("ListTokens")]
    public async Task<ActionResult> GetAllTokens(LoginRequest loginRequest)
    {
        (Boolean result, Account? selected) = await AccountUtils.Login(loginRequest);
        if (!result) return Unauthorized();
        
        // Get tokens
        IAccess access = new Access();
        const String sql = "SELECT * FROM tokens WHERE Username = @Username";
        List<TokenClass> tokens = await access.Load<TokenClass, dynamic>(sql, new
        {
            Username = selected!.Username
        }, AccountUtils.ConnectionString);
        return Ok(tokens);
    }

    [HttpPost]
    [Route("DeleteToken")]
    public async Task<ActionResult> DeleteToken(TokenClass token)
    {
        IAccess access = new Access();
        const String sql = "DELETE FROM tokens WHERE Token = @Token";
        await access.Save(sql, new
        {
            Token = token.Token
        }, AccountUtils.ConnectionString);
        return Ok();
    }

    [HttpPost]
    [Route("DeleteAllTokens")]
    public async Task<ActionResult> DeleteAllTokens(LoginRequest loginRequest)
    {
        (Boolean result, Account? selected) = await AccountUtils.Login(loginRequest);
        if (!result) return Unauthorized();
        
        // Delete all tokens
        IAccess access = new Access();
        const String sql = "DELETE FROM tokens WHERE Username = @Username";
        await access.Save(sql, new
        {
            Username = selected!.Username
        }, AccountUtils.ConnectionString);
        return Ok();
    }
}