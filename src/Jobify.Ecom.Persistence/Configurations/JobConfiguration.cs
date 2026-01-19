using Jobify.Ecom.Domain.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobify.Ecom.Persistence.Configurations;

internal class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);

        // Audit
        builder.OwnsOne(j => j.AuditState, audit =>
        {
            audit.Property(a => a.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired();

            audit.Property(a => a.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired();
        });

        // Properties
        builder.Property(j => j.Title)
               .IsRequired()
               .HasMaxLength(150);

        builder.Property(j => j.Description)
               .IsRequired()
               .HasMaxLength(3000);

        builder.Property(j => j.JobType)
               .IsRequired()
               .HasConversion<string>();

        builder.Property(j => j.MinSalary)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(j => j.MaxSalary)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(j => j.ClosingDate)
               .IsRequired();

        // Navigation: Job -> JobApplications
        builder.HasMany(j => j.JobApplications)
               .WithOne(ja => ja.Job)
               .HasForeignKey(ja => ja.JobId)
               .OnDelete(DeleteBehavior.Cascade);

        // Navigation: Job -> PostedByUser
        builder.HasOne(j => j.PostedByUser)
               .WithMany(u => u.Jobs)
               .HasForeignKey(j => j.PostedByUserId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
