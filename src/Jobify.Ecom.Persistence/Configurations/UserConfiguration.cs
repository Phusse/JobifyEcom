using Jobify.Ecom.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobify.Ecom.Persistence.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.SourceUserId)
            .IsRequired();

        builder.HasIndex(u => u.SourceUserId)
            .IsUnique();

        builder.HasMany(u => u.Jobs)
            .WithOne()
            .HasForeignKey(j => j.PostedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
