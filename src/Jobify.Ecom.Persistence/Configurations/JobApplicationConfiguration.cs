using Jobify.Ecom.Domain.Entities.JobApplications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobify.Ecom.Persistence.Configurations;

internal class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.HasKey(ja => ja.Id);

        builder.OwnsOne(ja => ja.AuditState, audit =>
        {
            audit.Property(a => a.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired();

            audit.Property(a => a.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired();
        });

        builder.Property(ja => ja.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.HasIndex(ja => new { ja.ApplicantUserId, ja.JobId })
            .IsUnique();
    }
}
