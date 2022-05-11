namespace SymmetricalWaifu.Shared;

public class WaifuDataTransfer
{
    public String Directory { get; set; } = String.Empty;
    public String Name { get; set; } = String.Empty;
    public String Extension { get; set; } = String.Empty;
    public Byte[] Contents { get; set; } = Array.Empty<Byte>();
}