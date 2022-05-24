namespace SymmetricalWaifu.Shared;

public class WaifuDataTransfer
{
    public string Directory { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public byte[] Contents { get; set; } = Array.Empty<byte>();
}