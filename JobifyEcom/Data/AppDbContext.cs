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
    /// Gets or sets the Skills table.
    /// </summary>
    public DbSet<Skill> Skills { get; set; }

    /// <summary>
    /// Gets or sets the JobPosts table.
    /// </summary>
    public DbSet<JobPost> JobPosts { get; set; }

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
    /// Gets or sets the SkillVerification table.
    /// </summary>
    public DbSet<SkillVerification> SkillVerifications { get; set; }


    /// <summary>
    /// Configures entity relationships and value conversions for the database schema.
    /// </summary>
    /// <param name="modelBuilder">
    /// The builder used to construct the model for the context.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureUser(modelBuilder);
        ConfigureWorkerProfile(modelBuilder);
        ConfigureSkill(modelBuilder);
        ConfigureJobPost(modelBuilder);
        ConfigureJobApplication(modelBuilder);
        ConfigureRating(modelBuilder);
        ConfigureTag(modelBuilder);
        ConfigureEntityTag(modelBuilder);
        ConfigureSkillVerification(modelBuilder);
    }

    private static void ConfigureUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Role)
                .HasConversion<string>();

            entity.HasIndex(u => u.Email)
                .IsUnique();
        });
    }

    private static void ConfigureWorkerProfile(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkerProfile>(entity =>
        {
            entity.HasOne(w => w.User)
                .WithOne(u => u.WorkerProfile)
                .HasForeignKey<WorkerProfile>(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(w => w.Skills)
                .WithOne(s => s.WorkerProfile)
                .HasForeignKey(s => s.WorkerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(w => w.JobPosts)
                .WithOne(j => j.Worker)
                .HasForeignKey(j => j.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSkill(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Skill>(entity =>
        {
            entity.Property(s => s.Level)
                .HasConversion<string>();

            entity.HasOne(s => s.WorkerProfile)
                .WithMany(w => w.Skills)
                .HasForeignKey(s => s.WorkerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(s => s.Verifications)
                .WithOne(v => v.Skill)
                .HasForeignKey(v => v.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureJobPost(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobPost>(entity =>
        {
            entity.Property(j => j.Status)
                .HasConversion<string>();

            entity.HasOne(j => j.Worker)
                .WithMany(w => w.JobPosts)
                .HasForeignKey(j => j.WorkerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureJobApplication(ModelBuilder modelBuilder)
    {
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

    private static void ConfigureRating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.WorkerProfile)
                .WithMany()
                .HasForeignKey(r => r.WorkerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(r => r.JobPost)
                .WithMany()
                .HasForeignKey(r => r.JobPostId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureTag(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasIndex(t => t.Name).IsUnique();

            entity.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasConversion(
                    v => v.ToLower(),
                    v => v
                );
        });
    }

    private static void ConfigureEntityTag(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EntityTag>(entity =>
        {
            entity.HasIndex(e => new { e.TagId, e.EntityId, e.EntityType }).IsUnique();

            entity.HasOne(e => e.Tag)
                  .WithMany(t => t.EntityTags)
                  .HasForeignKey(e => e.TagId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureSkillVerification(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SkillVerification>(entity =>
        {
            entity.Property(v => v.Status)
                .HasConversion<string>();

            entity.HasOne(v => v.Skill)
                .WithMany(s => s.Verifications)
                .HasForeignKey(v => v.SkillId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(v => v.ReviewedByUser)
                .WithMany()
                .HasForeignKey(v => v.ReviewedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
