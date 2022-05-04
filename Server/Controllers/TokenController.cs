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
        (Boolean result, Account? selected) = await UserUtils.Login(loginRequest);
        if (!result) return Unauthorized();
        String token = await UserUtils.NewToken(selected!.Username);
        return Ok(token);
    }
    
    [HttpPost]
    [Route("NewFromExistingToken")]
    public async Task<ActionResult> NewFromExistingToken([FromBody] String oldToken)
    {
        // Validate existing token
        (Boolean exists, String username) = await UserUtils.GetUnameFromToken(oldToken);
        
        // Validation
        if (exists is not true) return Unauthorized();
        
        // Generate new token
        String newToken = await UserUtils.NewToken(username);
        return Ok(newToken);
    }

    [HttpPost]
    [Route("ListTokens")]
    public async Task<ActionResult> GetAllTokens([FromBody] LoginRequest loginRequest)
    {
        (Boolean result, Account? selected) = await UserUtils.Login(loginRequest);
        if (!result) return Unauthorized();
        
        // Get tokens
        IAccess access = new Access();
        const String sql = "SELECT * FROM tokens WHERE Username = @Username";
        List<TokenClass> tokens = await access.Query<TokenClass, dynamic>(sql, new
        {
            Username = selected!.Username
        }, UserUtils.ConnectionString);
        return Ok(tokens);
    }

    [HttpPost]
    [Route("DeleteToken")]
    public async Task<ActionResult> DeleteToken([FromBody] String token)
    {
        IAccess access = new Access();
        const String sql = "DELETE FROM tokens WHERE Token = @Token LIMIT 1";
        await access.Execute(sql, new
        {
            Token = token
        }, UserUtils.ConnectionString);
        return Ok();
    }

    [HttpPost]
    [Route("DeleteAllTokens")]
    public async Task<ActionResult> DeleteAllTokens([FromBody] LoginRequest loginRequest)
    {
        (Boolean result, Account? selected) = await UserUtils.Login(loginRequest);
        if (!result) return Unauthorized();
        
        // Delete all tokens
        IAccess access = new Access();
        const String sql = "DELETE FROM tokens WHERE Username = @Username";
        await access.Execute(sql, new
        {
            Username = selected!.Username
        }, UserUtils.ConnectionString);
        return Ok();
    }
}