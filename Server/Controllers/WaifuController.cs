using DatabaseAccess;
using FileTypeChecker;
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
	public async Task<ActionResult> AllocateWaifu([FromBody] string token)
	{
		// Check if token is valid and get username if so
		(bool exists, string username) = await Utils.GetUnameFromToken(token);
		if (exists is not true) return Unauthorized();

		// Get directory or create one
		(bool create, string directory) = await Utils.CreateOrGetDirFromUname(username);
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
		string path = $"Waifus/{file.Directory}/{file.Name}{file.Extension}";

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
		string path = $"Waifus/{waifu.Directory}/{waifu.Name}{waifu.Extension}";
		await using (FileStream fs = System.IO.File.OpenRead(path))
		{
			bool isRecognizableType = FileTypeValidator.IsTypeRecognizable(fs);
			if (isRecognizableType is false || fs.IsImage() is false)
			{
				System.IO.File.Delete(path);
				return new StatusCodeResult(StatusCodes.Status418ImATeapot);
			}
		}

		// Add to waifu database
		IAccess access = new Access();
		const string sql =
			"INSERT INTO waifus (Id, ImageTitle, ImageDescription, ImagePath, Uploader, UploadDatetime, Votes, Origin) VALUES (@Id, @ImageTitle, @ImageDescription, @ImagePath, @Uploader, @UploadDatetime, @Votes, @Origin)";
		(bool valid, string username) = await Utils.GetUnameFromToken(waifu.Token);
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
		const string sql = "SELECT * FROM waifus";
		List<WaifuDbObject> waifus =
			await access.QueryAsync<WaifuDbObject, dynamic>(sql, new { }, Utils.ConnectionString);
		return Ok(waifus);
	}

	[HttpGet]
	[Route("GetAllPictures")]
	public async Task<ActionResult> GetAllPictures()
	{
		// Get list from database
		IAccess access = new Access();
		const string sql = "SELECT * FROM waifus";
		List<WaifuDbObject> waifuList =
			await access.QueryAsync<WaifuDbObject, dynamic>(sql, new { }, Utils.ConnectionString);
		
		// Generate list with images
		var waifus = new List<WaifuDownload>();
		foreach (WaifuDbObject waifu in waifuList)
		{
			waifus.Add(new WaifuDownload
			{
			});
		}

		return NotFound();
	}

	[HttpGet]
	[Route("GetWaifu/{id}")]
	public async Task<ActionResult> GetWaifuById(string id)
	{
		IAccess access = new Access();
		const string sql = "SELECT * FROM waifus WHERE Id = @Id LIMIT 1";
		List<WaifuDbObject> waifu = await access.QueryAsync<WaifuDbObject, dynamic>(sql, new
		{
			Id = id
		}, Utils.ConnectionString);
		if (waifu.Count is 0) return NotFound();
		
		string path = waifu.First().ImagePath;
		byte[] bytes = await System.IO.File.ReadAllBytesAsync(path);

		string extension = Path.GetExtension(path);
		WaifuDownload result = extension switch
		{
			".png" => new WaifuDownload { Bytes = bytes, IsPngOrJpeg = true },
			".jpeg" => new WaifuDownload { Bytes = bytes, IsPngOrJpeg = true },
			".jpg" => new WaifuDownload { Bytes = bytes, IsPngOrJpeg = true },
			_ => new WaifuDownload { Bytes = bytes, IsPngOrJpeg = false },
		};

		return Ok(result);
	}
}