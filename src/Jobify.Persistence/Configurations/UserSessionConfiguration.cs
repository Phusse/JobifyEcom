using Jobify.Domain.Entities.UserSessions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobify.Persistence.Configurations;

internal class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.HasKey(us => us.Id);

        builder.OwnsOne(us => us.AuditState, audit =>
        {
            audit.Property(a => a.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired();

            audit.Property(a => a.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired();
        });

        builder.HasIndex(us => us.UserId);

        builder.Property(us => us.UserId)
            .IsRequired();

        builder.HasOne(us => us.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(us => us.UserId)
            .IsRequired();

        builder.Property(us => us.RememberMe)
            .IsRequired();

        builder.Property(us => us.IsRevoked)
            .IsRequired();

        builder.Property(us => us.ExpiresAt)
            .IsRequired();

        builder.Property(us => us.AbsoluteExpiresAt)
            .IsRequired();
    }
}
