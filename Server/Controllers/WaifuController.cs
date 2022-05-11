using System.Security.Cryptography;
using DatabaseAccess;
using FileTypeChecker;
using FileTypeChecker.Abstracts;
using FileTypeChecker.Extensions;
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
        var filePath = new WaifuAllocation
        {
            Directory = directory,
            NameWithoutExtension = Guid.NewGuid().ToString("D")
        };
        return Ok(filePath);
    }

    [HttpPost]
    [Route("UploadWaifu")]
    public async Task<ActionResult> UploadWaifu([FromBody] WaifuDataTransfer file)
    {
        // Initialization
        if (Directory.Exists($"Waifus/{file.Directory}") is false) return Unauthorized();
        String path = $"Waifus/{file.Directory}/{file.Name}{file.Extension}";

        // Write bytes
        await using (var fs = new FileStream(path, FileMode.Append))
        {
            fs.Write(file.Contents, 0, file.Contents.Length);
        }

        return Ok();
    }

    [HttpPost]
    [Route("SubmitWaifu")]
    public async Task<ActionResult> SubmitWaifu([FromBody] WaifuSubmission waifu)
    {
        // Check file type
        String path = $"Waifus/{waifu.Directory}/{waifu.Name}{waifu.Extension}";
        await using (FileStream fs = System.IO.File.OpenRead(path))
        {
            Boolean isRecognizableType = FileTypeValidator.IsTypeRecognizable(fs);
            if (isRecognizableType is false || fs.IsImage() is false)
            {
                System.IO.File.Delete(path);
                return new StatusCodeResult(StatusCodes.Status418ImATeapot);
            }
        }

        // Add to waifu database
        IAccess access = new Access();
        const String sql =
            "INSERT INTO waifus (Id, ImageTitle, ImageDescription, ImagePath, Uploader, UploadDatetime, Votes, Origin) VALUES (@Id, @ImageTitle, @ImageDescription, @ImagePath, @Uploader, @UploadDatetime, @Votes, @Origin)";
        (Boolean valid, String username) = await Utils.GetUnameFromToken(waifu.Token);
        if (valid is false) return Unauthorized();
        await access.ExecuteAsync(sql, new
        {
            Id = Utils.GenId(),
            ImageTitle = waifu.Title,
            ImageDescription = waifu.Description,
            ImagePath = path,
            Uploader = username,
            UploadDatetime = DateTime.UtcNow,
            Votes = 0,
            Origin = waifu.Source
        }, Utils.ConnectionString);

        return Ok();
    }

    [HttpGet]
    [Route("GetAllWaifus")]
    public async Task<ActionResult> GetAllWaifus()
    {
        IAccess access = new Access();
        const String sql = "SELECT * FROM waifus";
        List<WaifuObject> waifus = await access.QueryAsync<WaifuObject, dynamic>(sql, new { }, Utils.ConnectionString);
        return Ok(waifus);
    }
}