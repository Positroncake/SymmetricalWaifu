namespace SymmetricalWaifu.Shared;

public class WaifuObject
{
    public String Id { get; set; } = String.Empty;
    public String ImageTitie { get; set; } = String.Empty;
    public String ImageDescription { get; set; } = String.Empty;
    public String ImagePath { get; set; } = String.Empty;
    public String Uploader { get; set; } = String.Empty;
    public DateTime UploadDatetime { get; set; }
    public Int32 Votes { get; set; }
    public String Origin { get; set; } = String.Empty;
}