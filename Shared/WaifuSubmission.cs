namespace SymmetricalWaifu.Shared;

public class WaifuSubmission
{
    #region File info

    public string Directory { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;

    #endregion

    #region Waifu info

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;

    #endregion
}