using JobifyEcom.DTOs;
using JobifyEcom.Models;

namespace JobifyEcom.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto);
    Task<User> ConfirmEmailAsync(string email, Guid token);
    Task<User> GetUserByEmailAsync(string email);
}