using Jobify.Domain.Abstractions;
using Jobify.Domain.Components.Auditing;
using Jobify.Domain.Components.Security;
using Jobify.Domain.Entities.UserSessions;
using Jobify.Domain.Enums;

namespace Jobify.Domain.Entities.Users;

public class User : IEntity, IAuditable, IHasSensitiveData<UserSensitive>
{
    internal readonly AuditState AuditState = new();
    internal readonly SensitiveDataState<UserSensitive> SensitiveDataState = new();

    private User() { }

    public User(string userName, string emailHash, string passwordHash)
    {
        SetUserName(userName);
        SetEmailHash(emailHash);
        SetPasswordHash(passwordHash);
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public DateTime CreatedAt => AuditState.CreatedAt;
    public DateTime UpdatedAt => AuditState.UpdatedAt;

    public byte[] EncryptedData => SensitiveDataState.EncryptedData;
    public UserSensitive? SensitiveData => SensitiveDataState.SensitiveData;

    public string UserName { get; private set; } = string.Empty;
    public string EmailHash { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsLocked { get; private set; } = false;
    public SystemRole Role { get; private set; } = SystemRole.User;

    public ICollection<UserSession> Sessions { get; private set; } = [];

    public void SetEncryptedData(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        if (data.Length == 0)
            throw new ArgumentException("Data cannot be empty.", nameof(data));

        SensitiveDataState.SetEncryptedData(data);
        AuditState.UpdateAudit();
    }

    public void SetSensitiveData(UserSensitive data) => SensitiveDataState.SetSensitiveData(data);
    public void ClearSensitiveData() => SensitiveDataState.ClearSensitiveData();

    public void SetUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("UserName cannot be null or empty.", nameof(userName));

        UserName = userName;
        AuditState.UpdateAudit();
    }

    public void SetEmailHash(string emailHash)
    {
        if (string.IsNullOrWhiteSpace(emailHash))
            throw new ArgumentException("EmailHash cannot be null or empty.", nameof(emailHash));

        EmailHash = emailHash;
        AuditState.UpdateAudit();
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("PasswordHash cannot be null or empty.", nameof(passwordHash));

        PasswordHash = passwordHash;
        AuditState.UpdateAudit();
    }
}
