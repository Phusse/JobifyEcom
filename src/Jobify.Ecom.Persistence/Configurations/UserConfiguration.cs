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

        // Navigation: User -> Jobs
        builder.HasMany(u => u.Jobs)
               .WithOne(j => j.PostedByUser)      // navigation property on Job
               .HasForeignKey(j => j.PostedByUserId)
               .OnDelete(DeleteBehavior.Restrict);

        // Navigation: User -> JobApplications
        builder.HasMany(u => u.JobApplications)
               .WithOne(ja => ja.ApplicantUser)
               .HasForeignKey(ja => ja.ApplicantUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
