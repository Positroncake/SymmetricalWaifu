using Microsoft.AspNetCore.Mvc;
using SymmetricalWaifu.Shared;

namespace SymmetricalWaifu.Server.Controllers;

[ApiController]
[Route("WaifuApi")]
public class WaifuController : ControllerBase
{
    [HttpPost]
    [Route("AllocateWaifu")]
    public async Task<ActionResult> AllocateWaifu(String token)
    {
        // Check if token is valid and get username if so
        (Boolean exists, String username) = await UserUtils.GetUnameFromToken(token);
        if (exists is not true) return Unauthorized();

        // Get directory or create one
        (Boolean create, String directory) = await UserUtils.CreateOrGetDirFromUname(username);
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
    public async Task<ActionResult> UploadWaifu(FilePacket file)
    {
        // Init
        if (Directory.Exists(file.Directory) is false) return Unauthorized();
        String path = $"Waifus/{file.Directory}/{file.Name}.{file.Extension}";
        
        // Write bytes
        await using (var fs = new FileStream(path, FileMode.Append))
        {
            fs.Write(file.Contents, 0, file.Contents.Length);
        }
        return Ok();
    }
}