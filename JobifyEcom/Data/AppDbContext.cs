using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Data;

/// <summary>
/// Represents the application's database context used to interact with the underlying database.
/// Configures entity mappings and relationships between models.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the Users table.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the WorkerProfiles table.
    /// </summary>
    public DbSet<WorkerProfile> WorkerProfiles { get; set; }

    /// <summary>
    /// Gets or sets the JobPosts table.
    /// </summary>
    public DbSet<JobPost> JobPosts { get; set; }

    /// <summary>
    /// Gets or sets the JobApplications table.
    /// </summary>
    public DbSet<JobApplication> JobApplications { get; set; }

    /// <summary>
    /// Configures entity relationships and value conversions for the database schema.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for the context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Role)
                .HasConversion<string>();
        });

        // Configure WorkerProfile entity
        modelBuilder.Entity<WorkerProfile>(entity =>
        {
            entity.HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure JobPost entity
        modelBuilder.Entity<JobPost>(entity =>
        {
            entity.Property(j => j.Status)
                .HasConversion<string>();

            entity.HasOne(j => j.Worker)
                .WithMany()
                .HasForeignKey(j => j.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure JobApplication entity
        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.Property(j => j.Status)
                .HasConversion<string>();

            entity.HasOne(j => j.Customer)
                .WithMany()
                .HasForeignKey(j => j.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(j => j.JobPost)
                .WithMany()
                .HasForeignKey(j => j.JobPostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
