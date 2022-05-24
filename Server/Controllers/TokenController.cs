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
    public async Task<ActionResult> NewFromLoginDetails([FromBody] LoginRequest loginRequest)
    {
        (bool result, Account? selected) = await Utils.Login(loginRequest);
        if (!result) return Unauthorized();
        string token = await Utils.NewToken(selected!.Username);
        return Ok(token);
    }
    
    [HttpPost]
    [Route("NewFromExistingToken")]
    public async Task<ActionResult> NewFromExistingToken([FromBody] string oldToken)
    {
        // Validate existing token
        (bool exists, string username) = await Utils.GetUnameFromToken(oldToken);
        
        // Validation
        if (exists is not true) return Unauthorized();
        
        // Generate new token
        string newToken = await Utils.NewToken(username);
        return Ok(newToken);
    }

    [HttpPost]
    [Route("ListTokens")]
    public async Task<ActionResult> GetAllTokens([FromBody] LoginRequest loginRequest)
    {
        (bool result, Account? selected) = await Utils.Login(loginRequest);
        if (!result) return Unauthorized();
        
        // Get tokens
        IAccess access = new Access();
        const string sql = "SELECT * FROM tokens WHERE Username = @Username";
        List<TokenClass> tokens = await access.QueryAsync<TokenClass, dynamic>(sql, new
        {
            selected!.Username
        }, Utils.ConnectionString);
        return Ok(tokens);
    }

    [HttpPost]
    [Route("DeleteToken")]
    public async Task<ActionResult> DeleteToken([FromBody] string token)
    {
        IAccess access = new Access();
        const string sql = "DELETE FROM tokens WHERE Token = @Token LIMIT 1";
        await access.ExecuteAsync(sql, new
        {
            Token = token
        }, Utils.ConnectionString);
        return Ok();
    }

    [HttpPost]
    [Route("DeleteAllTokens")]
    public async Task<ActionResult> DeleteAllTokens([FromBody] LoginRequest loginRequest)
    {
        (bool result, Account? selected) = await Utils.Login(loginRequest);
        if (!result) return Unauthorized();
        
        // Delete all tokens
        IAccess access = new Access();
        const string sql = "DELETE FROM tokens WHERE Username = @Username";
        await access.ExecuteAsync(sql, new
        {
            selected!.Username
        }, Utils.ConnectionString);
        return Ok();
    }
}