using Jobify.Ecom.Domain.Abstractions;

namespace Jobify.Ecom.Domain.Entities.Users;

public class User : IEntity
{
    private User() { }

    public User(Guid sourceUserId)
        => SourceUserId = sourceUserId;

    public Guid Id { get; private set; } = Guid.CreateVersion7();

    public Guid SourceUserId { get; private set; }
}
