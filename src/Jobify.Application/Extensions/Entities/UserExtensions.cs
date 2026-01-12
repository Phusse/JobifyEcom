using Jobify.Application.Features.Users.Models;
using Jobify.Domain.Entities.Users;

namespace Jobify.Application.Extensions.Entities;

internal static class UserExtensions
{
    extension(User user)
    {
        public UserResponse ToUserResponse()
        {
            if (user.SensitiveData is null)
                throw new InvalidOperationException("User sensitive data has not been decrypted.");

            UserSensitive data = user.SensitiveData;

            return new UserResponse(
                user.Id,
                user.UserName,
                data.FirstName,
                data.MiddleName ?? string.Empty,
                data.LastName,
                user.Role
            );
        }
    }
}
