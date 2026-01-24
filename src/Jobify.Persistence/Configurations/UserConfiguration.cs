using Jobify.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobify.Persistence.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.OwnsOne(u => u.AuditState, audit =>
        {
            audit.Property(a => a.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired();

            audit.Property(a => a.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired();
        });

        builder.OwnsOne(u => u.SensitiveDataState, sensitive =>
        {
            sensitive.Property(s => s.EncryptedData)
                .HasColumnName("Data")
                .IsRequired();

            sensitive.Ignore(s => s.SensitiveData);
        });

        builder.HasIndex(u => u.UserName)
            .IsUnique();

        builder.Property(u => u.UserName)
            .HasMaxLength(UserLimits.UserNameMaxLength)
            .IsRequired();

        builder.HasIndex(u => u.EmailHash)
            .IsUnique();

        builder.Property(u => u.EmailHash)
            .HasMaxLength(UserLimits.EmailHashMaxLength)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(UserLimits.PasswordHashMaxLength)
            .IsRequired();

        builder.Property(u => u.IsLocked)
            .IsRequired();

        builder.Property(u => u.Role)
            .HasConversion<string>()
            .IsRequired();

        builder.HasMany(u => u.Sessions)
            .WithOne(us => us.User)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
