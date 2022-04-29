namespace SymmetricalWaifu.Shared;

public class LoginRequest
{
    public String Username { get; set; } = String.Empty;
    public Byte[] Password { get; set; } = Array.Empty<Byte>(); // 64 Bytes (512-bit)
}