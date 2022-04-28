using DatabaseAccess;
using Microsoft.AspNetCore.Mvc;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server.Controllers;

[ApiController]
[Route("AccountApi")]
public class AccountController : ControllerBase
{
    private List<Account>? _accounts;
    private const String ConnectionString =
        "Server=127.0.0.1;Port=3306;Database=symmetrical_waifu;Uid=waifudatabase;Pwd=JUJqzeFfrUozFV5Wpxuxh3mSwXzrsPq7";
    
    [HttpPost]
    [Route("Register")]
    public ActionResult Register()
    {
        return new StatusCodeResult(StatusCodes.Status501NotImplemented);
    }

    [HttpPost]
    [Route("Login")]
    public ActionResult Login()
    {
        return new StatusCodeResult(StatusCodes.Status501NotImplemented);
    }

    [HttpGet]
    [Route("GetAccounts")]
    public async Task<ActionResult> Get()
    {
        IAccess access = new Access();
        const String sql = "SELECT * FROM symmetrical_waifu.accounts";
        _accounts = await access.Load<Account, dynamic>(sql, new { }, ConnectionString);
        return Ok(_accounts);
    }
}