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

        builder.HasMany(u => u.PostedJobs)
            .WithOne(j => j.PostedByUser)
            .HasForeignKey(j => j.PostedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.JobApplications)
            .WithOne(ja => ja.ApplicantUser)
            .HasForeignKey(ja => ja.ApplicantUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
