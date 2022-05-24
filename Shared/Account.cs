namespace SymmetricalWaifu.Shared;

public class Account
{
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>(); // 64 Bytes (512-bit)
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>(); // 128 Bytes (1024-bit)
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime Joined { get; set; }
    public int Submissions { get; set; }
    public int WinningSubmissions { get; set; }
}