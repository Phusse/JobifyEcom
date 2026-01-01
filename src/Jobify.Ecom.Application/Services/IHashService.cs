namespace Jobify.Ecom.Application.Services;

public interface IHashingService
{
	Task<string> HashPasswordAsync(string password);
	Task<bool> VerifyPasswordAsync(string password, string hashedPassword);

	string HashEmail(string email);
	bool VerifyEmail(string email, string hashedEmail);
}
