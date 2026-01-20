using Jobify.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobify.Persistence.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.UserName)
            .IsUnique();

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

        builder.Property(u => u.UserName)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(u => u.EmailHash)
            .IsUnique();

        builder.Property(u => u.EmailHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(60)
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
