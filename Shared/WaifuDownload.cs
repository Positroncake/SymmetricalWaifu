namespace SymmetricalWaifu.Shared;

public class WaifuDownload
{
	public string? Id { get; set; } = null;
	public string? ImageTitle { get; set; } = null;
	public string? ImageDescription { get; set; } = null;

	public byte[] Bytes { get; set; } = Array.Empty<byte>();
	public bool IsPngOrJpeg { get; set; }

	public string? Uploader { get; set; } = null;
	public DateTime? UploadDatetime { get; set; } = null;
	public int? Votes { get; set; } = null;
	public string? Origin { get; set; } = null;
}