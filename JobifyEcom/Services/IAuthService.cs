public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
    Task<User> ConfirmEmailAsync(string email, string token);
    Task<User> GetUserByEmailAsync(string email);
}