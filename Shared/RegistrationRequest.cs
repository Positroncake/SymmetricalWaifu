namespace SymmetricalWaifu.Shared;

public class RegistrationRequest
{
    public String Username { get; set; } = String.Empty;
    public Byte[] PasswordHash { get; set; } = Array.Empty<Byte>(); // 64 Bytes (512-bit)
    public Byte[] PasswordSalt { get; set; } = Array.Empty<Byte>(); // 128 Bytes (1024-bit)
    public String DisplayName { get; set; } = String.Empty;
    public String Email { get; set; } = String.Empty;
}