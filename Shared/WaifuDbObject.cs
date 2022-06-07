namespace SymmetricalWaifu.Shared;

public class WaifuDbObject
{
    public string Id { get; set; } = string.Empty;
    public string ImageTitle { get; set; } = string.Empty;
    public string ImageDescription { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public string Uploader { get; set; } = string.Empty;
    public DateTime UploadDatetime { get; set; }
    public int Votes { get; set; }
    public string Origin { get; set; } = string.Empty;
}