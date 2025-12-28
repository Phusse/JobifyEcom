using Jobify.Ecom.Domain.Components.Auditing;
using Jobify.Ecom.Domain.Components.Security;
using Jobify.Ecom.Domain.Entities.UserSessions;
using Jobify.Ecom.Domain.Enums;
using Jobify.Ecom.Domain.Extensions.Validations;

namespace Jobify.Ecom.Domain.Entities.Users;

public class User
{
    private readonly AuditState _audit = new();
    private readonly SensitiveDataState<UserSensitive> _sensitive = new();

    public static string Audit => nameof(_audit);
    public static string Sensitive => nameof(_sensitive);

    private User() { }

    public User(string userName, string emailHash, string passwordHash)
    {
        UserName = userName.EnsureNotEmptyAndTrim();
        EmailHash = emailHash.EnsureNotEmptyAndTrim();
        PasswordHash = passwordHash.EnsureNotEmptyAndTrim();

        _audit.UpdateAudit();
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public string UserName { get; private set; } = null!;
    public string EmailHash { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;

    public bool IsLocked { get; private set; } = false;
    public SystemRole Role { get; private set; } = SystemRole.User;

    public DateTime CreatedAt => _audit.CreatedAt;
    public DateTime UpdatedAt => _audit.UpdatedAt;

    public byte[] EncryptedData => _sensitive.EncryptedData;
    public UserSensitive? SensitiveData => _sensitive.SensitiveData;

    public ICollection<UserSession> Sessions { get; private set; } = [];

    public void ChangeRole(SystemRole newRole)
    {
        if (Role == newRole) return;

        if (!Enum.IsDefined(newRole))
            throw new ArgumentException("Invalid role.");

        Role = newRole;
        _audit.UpdateAudit();
    }

    public void UpdateUserName(string newUserName)
    {
        if (newUserName == UserName) return;

        UserName = newUserName.EnsureNotEmptyAndTrim();
        _audit.UpdateAudit();
    }

    public void UpdateEmailHash(string newEmailHash)
    {
        if (newEmailHash == EmailHash) return;

        EmailHash = newEmailHash.EnsureNotEmptyAndTrim();
        _audit.UpdateAudit();
    }

    public void UpdatePasswordHash(string newPasswordHash)
    {
        if (newPasswordHash == PasswordHash) return;

        PasswordHash = newPasswordHash.EnsureNotEmptyAndTrim();
        _audit.UpdateAudit();
    }

    public void LockAccount()
    {
        if (IsLocked) return;

        IsLocked = true;
        _audit.UpdateAudit();
    }

    public void UnlockAccount()
    {
        if (!IsLocked) return;

        IsLocked = false;
        _audit.UpdateAudit();
    }

    public void SetSensitiveData(UserSensitive data) => _sensitive.SetSensitiveData(data);
    public void SetEncryptedData(byte[] data) => _sensitive.SetEncryptedData(data);
    public void ClearDecryptedData() => _sensitive.ClearSensitiveData();
    public void ClearEncryptedData() => _sensitive.ClearEncryptedData();
}
