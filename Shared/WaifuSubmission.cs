namespace SymmetricalWaifu.Shared;

public class WaifuSubmission
{
    #region File info

    public String Directory { get; set; } = String.Empty;
    public String Name { get; set; } = String.Empty;
    public String Extension { get; set; } = String.Empty;

    #endregion

    #region Waifu info

    public String Title { get; set; } = String.Empty;
    public String Description { get; set; } = String.Empty;
    public String Token { get; set; } = String.Empty;
    public String Source { get; set; } = String.Empty;

    #endregion
}