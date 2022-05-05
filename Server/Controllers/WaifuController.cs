using Microsoft.AspNetCore.Mvc;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server.Controllers;

[ApiController]
[Route("WaifuApi")]
public class WaifuController : ControllerBase
{
    [HttpPost]
    [Route("AllocateWaifu")]
    public async Task<ActionResult> AllocateWaifu([FromBody] String token)
    {
        // Check if token is valid and get username if so
        (Boolean exists, String username) = await Utils.GetUnameFromToken(token);
        if (exists is not true) return Unauthorized();

        // Get directory or create one
        (Boolean create, String directory) = await Utils.CreateOrGetDirFromUname(username);
        if (create) Directory.CreateDirectory($"Waifus/{directory}");

        // Return info to user
        var filePath = new FilePath
        {
            Directory = directory,
            NameWithoutExtension = Guid.NewGuid().ToString("D")
        };
        return Ok(filePath);
    }
    
    [HttpPost]
    [Route("UploadWaifu")]
    public async Task<ActionResult> UploadWaifu([FromBody] FilePacket file)
    {
        // Init
        if (Directory.Exists($"Waifus/{file.Directory}") is false) return Unauthorized();
        String path = $"Waifus/{file.Directory}/{file.Name}.{file.Extension}";
        
        // Write bytes
        await using (var fs = new FileStream(path, FileMode.Append))
        {
            fs.Write(file.Contents, 0, file.Contents.Length);
        }
        return Ok();
    }
}