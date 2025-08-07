namespace JobifyEcom.DTOs;

public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public AuthData? Data { get; set; }
}
