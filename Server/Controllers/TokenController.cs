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
        (Boolean result, Account? selected) = await Utils.Login(loginRequest);
        if (!result) return Unauthorized();
        String token = await Utils.NewToken(selected!.Username);
        return Ok(token);
    }
    
    [HttpPost]
    [Route("NewFromExistingToken")]
    public async Task<ActionResult> NewFromExistingToken([FromBody] TokenClass existingToken)
    {
        // Validate existing token
        (Boolean exists, String username) = await Utils.GetUnameFromToken(existingToken.Token);
        
        // Validation
        if (exists is not true) return Unauthorized();
        
        // Generate new token
        String token = await Utils.NewToken(username);
        return Ok(token);
    }

    [HttpPost]
    [Route("ListTokens")]
    public async Task<ActionResult> GetAllTokens([FromBody] LoginRequest loginRequest)
    {
        (Boolean result, Account? selected) = await Utils.Login(loginRequest);
        if (!result) return Unauthorized();
        
        // Get tokens
        IAccess access = new Access();
        const String sql = "SELECT * FROM tokens WHERE Username = @Username";
        List<TokenClass> tokens = await access.Load<TokenClass, dynamic>(sql, new
        {
            Username = selected!.Username
        }, Utils.ConnectionString);
        return Ok(tokens);
    }

    [HttpPost]
    [Route("DeleteToken")]
    public async Task<ActionResult> DeleteToken([FromBody] TokenClass token)
    {
        IAccess access = new Access();
        const String sql = "DELETE FROM tokens WHERE Token = @Token LIMIT 1";
        await access.Save(sql, new
        {
            Token = token.Token
        }, Utils.ConnectionString);
        return Ok();
    }

    [HttpPost]
    [Route("DeleteAllTokens")]
    public async Task<ActionResult> DeleteAllTokens([FromBody] LoginRequest loginRequest)
    {
        (Boolean result, Account? selected) = await Utils.Login(loginRequest);
        if (!result) return Unauthorized();
        
        // Delete all tokens
        IAccess access = new Access();
        const String sql = "DELETE FROM tokens WHERE Username = @Username";
        await access.Save(sql, new
        {
            Username = selected!.Username
        }, Utils.ConnectionString);
        return Ok();
    }
}