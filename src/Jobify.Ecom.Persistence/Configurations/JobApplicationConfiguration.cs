using Jobify.Ecom.Domain.Entities.JobApplications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobify.Ecom.Persistence.Configurations;

internal class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.HasKey(ja => ja.Id);

        // Audit
        builder.OwnsOne(ja => ja.AuditState, audit =>
        {
            audit.Property(a => a.CreatedAt)
                 .HasColumnName("CreatedAt")
                 .IsRequired();

            audit.Property(a => a.UpdatedAt)
                 .HasColumnName("UpdatedAt")
                 .IsRequired();
        });

        // Enum
        builder.Property(ja => ja.Status)
               .HasConversion<string>()
               .IsRequired();

        // Relationships
        builder.HasOne(ja => ja.Job)
               .WithMany(j => j.JobApplications)
               .HasForeignKey(ja => ja.JobId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ja => ja.ApplicantUser)
               .WithMany(u => u.JobApplications)
               .HasForeignKey(ja => ja.ApplicantUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
