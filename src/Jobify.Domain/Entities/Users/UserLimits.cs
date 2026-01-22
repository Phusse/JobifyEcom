namespace Jobify.Domain.Entities.Users;

public static class UserLimits
{
    public const int UserNameMinLength = 3;
    public const int UserNameMaxLength = 30;

    public const int EmailHashMaxLength = 64;

    public const int PasswordMinLength = 8;
    public const int PasswordHashMaxLength = 60;

    public const int NameMinLength = 1;
    public const int NameMaxLength = 40;

    public const int IdentifierIdMaxLength = 254;
}
