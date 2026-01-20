using Jobify.Ecom.Domain.Entities.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobify.Ecom.Persistence.Configurations;

internal class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.HasKey(j => j.Id);

        builder.OwnsOne(j => j.AuditState, audit =>
        {
            audit.Property(a => a.CreatedAt)
                .HasColumnName("CreatedAt")
                .IsRequired();

            audit.Property(a => a.UpdatedAt)
                .HasColumnName("UpdatedAt")
                .IsRequired();
        });

        builder.Property(j => j.Title)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(j => j.Description)
            .HasMaxLength(3000)
            .IsRequired();

        builder.Property(j => j.JobType)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(j => j.MinSalary)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(j => j.MaxSalary)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(j => j.ClosingDate)
            .IsRequired();

        builder.HasMany(j => j.Applications)
            .WithOne(ja => ja.Job)
            .HasForeignKey(ja => ja.JobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
