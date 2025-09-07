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
    public DbSet<Worker> Workers { get; set; }

    /// <summary>
    /// Gets or sets the Skills table.
    /// </summary>
    public DbSet<Skill> Skills { get; set; }

    /// <summary>
    /// Gets or sets the Job table.
    /// </summary>
    public DbSet<Job> Jobs { get; set; }

    /// <summary>
    /// Gets or sets the JobApplications table.
    /// </summary>
    public DbSet<JobApplication> JobApplications { get; set; }

    /// <summary>
    /// Gets or sets the Ratings table.
    /// </summary>
    public DbSet<Rating> Ratings { get; set; }

    /// <summary>
    /// Gets or sets the Tags table.
    /// </summary>
    public DbSet<Tag> Tags { get; set; }

    /// <summary>
    /// Gets or sets the EntityTags table that links tags to different entity types (JobPost, Skill, etc.).
    /// </summary>
    public DbSet<EntityTag> EntityTags { get; set; }

    /// <summary>
    /// Gets or sets the Verification table that hold verificatioins for different entities.
    /// </summary>
    public DbSet<Verification> Verifications { get; set; }

    /// <summary>
    /// Configures entity relationships and value conversions for the database schema.
    /// </summary>
    /// <param name="modelBuilder">
    /// The builder used to construct the model for the context.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);
        ConfigureWorker(modelBuilder);
        ConfigureSkill(modelBuilder);
        ConfigureJobPost(modelBuilder);
        ConfigureJobApplication(modelBuilder);
        ConfigureRating(modelBuilder);
        ConfigureTag(modelBuilder);
        ConfigureEntityTag(modelBuilder);
        ConfigureVerification(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            // Convert enum to string
            entity.Property(u => u.StaffRole)
                .HasConversion<string>();

            // Email must be unique
            entity.HasIndex(u => u.Email)
                .IsUnique();

            // One-to-one with Workers profile
            entity.HasOne(u => u.WorkerProfile)
                .WithOne(w => w.User)
                .HasForeignKey<Worker>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Jobs posted by this user
            entity.HasMany(u => u.JobsPosted)
                .WithOne(j => j.PostedBy)
                .HasForeignKey(j => j.PostedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ratings submitted by this user
            entity.HasMany(u => u.RatingsSubmitted)
                .WithOne(r => r.Reviewer)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureWorker(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Worker>(entity =>
        {
            // One-to-one with User
            entity.HasOne(w => w.User)
                .WithOne(u => u.WorkerProfile)
                .HasForeignKey<Worker>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Skills owned by the worker
            entity.HasMany(w => w.Skills)
                .WithOne(s => s.Worker)
                .HasForeignKey(s => s.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Job applications submitted by this worker
            entity.HasMany(w => w.ApplicationsSubmitted)
                .WithOne(ja => ja.Applicant)
                .HasForeignKey(ja => ja.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ratings received by this worker
            entity.HasMany(w => w.RatingsReceived)
                .WithOne(r => r.Worker)
                .HasForeignKey(r => r.WorkerProfileId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSkill(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Skill>(entity =>
        {
            // Convert enum to string
            entity.Property(s => s.Level)
                .HasConversion<string>();

            // One-to-many: Worker owns many skills
            entity.HasOne(s => s.Worker)
                .WithMany(w => w.Skills)
                .HasForeignKey(s => s.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for quick lookup by name and level if needed
            entity.HasIndex(s => new { s.Name, s.WorkerId })
                .IsUnique();
        });
    }

    private static void ConfigureJobPost(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>(entity =>
        {
            // Convert JobStatus enum to string
            entity.Property(j => j.Status)
                .HasConversion<string>();

            // Index on status for filtering
            entity.HasIndex(j => j.Status);

            // Job posted by a User
            entity.HasOne(j => j.PostedBy)
                .WithMany(u => u.JobsPosted)
                .HasForeignKey(j => j.PostedByUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Applications
            entity.HasMany(j => j.ApplicationsReceived)
                .WithOne(ja => ja.Job)
                .HasForeignKey(ja => ja.JobPostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ratings related to this job post
            entity.HasMany(j => j.RatingsReceived)
                .WithOne(r => r.Job)
                .HasForeignKey(r => r.JobPostId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureJobApplication(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobApplication>(entity =>
        {
            // Convert JobApplicationStatus enum to string
            entity.Property(ja => ja.Status)
                .HasConversion<string>();

            // Optional index for filtering by status
            entity.HasIndex(ja => ja.Status);

            // Worker who applied
            entity.HasOne(ja => ja.Applicant)
                .WithMany(w => w.ApplicationsSubmitted)
                .HasForeignKey(ja => ja.WorkerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Job post being applied to
            entity.HasOne(ja => ja.Job)
                .WithMany(j => j.ApplicationsReceived)
                .HasForeignKey(ja => ja.JobPostId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureRating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rating>(entity =>
        {
            // Customer submitting the rating
            entity.HasOne(r => r.Reviewer)
                .WithMany(u => u.RatingsSubmitted)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Worker profile being rated
            entity.HasOne(r => r.Worker)
                .WithMany(w => w.RatingsReceived)
                .HasForeignKey(r => r.WorkerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional JobPost reference
            entity.HasOne(r => r.Job)
                .WithMany(j => j.RatingsReceived)
                .HasForeignKey(r => r.JobPostId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureTag(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>(entity =>
        {
            // Enforce unique tag names
            entity.HasIndex(t => t.Name).IsUnique();

            // Normalize name to lowercase
            entity.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion(
                    v => v.ToLower(),  // store as lowercase
                    v => v             // read as-is
                );

            // Navigation to EntityTags
            entity.HasMany(t => t.TaggedEntities)
                .WithOne(et => et.Tag)
                .HasForeignKey(et => et.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }


    private static void ConfigureEntityTag(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EntityTag>(entity =>
        {
            // Convert EntityType enum to string in DB
            entity.Property(e => e.EntityType)
                .HasConversion<string>();

            // Ensure uniqueness for a tag on a given entity
            entity.HasIndex(e => new { e.TagId, e.EntityId, e.EntityType })
                .IsUnique();

            // Relationship to Tag
            entity.HasOne(e => e.Tag)
                .WithMany(t => t.TaggedEntities)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureVerification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Verification>(entity =>
        {
            // Convert enum to string
            entity.Property(v => v.EntityType)
                .HasConversion<string>();

            entity.Property(v => v.Status)
                .HasConversion<string>();

            // Index for faster lookups by status
            entity.HasIndex(v => v.Status);

            // Composite index to quickly find verification for an entity
            entity.HasIndex(v => new { v.EntityType, v.EntityId });

            // Relation to admin user who reviewed
            entity.HasOne(v => v.Reviewer)
                .WithMany()
                .HasForeignKey(v => v.ReviewerId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
