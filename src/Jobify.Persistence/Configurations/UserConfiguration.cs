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
            .IsRequired()
            .HasMaxLength(30);

        builder.HasIndex(u => u.EmailHash)
            .IsUnique();

        builder.Property(u => u.EmailHash)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(u => u.IsLocked)
            .IsRequired();

        builder.Property(u => u.Role)
            .IsRequired()
            .HasConversion<string>();

        builder.HasMany(u => u.Sessions)
            .WithOne(us => us.User)
            .HasForeignKey(us => us.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
