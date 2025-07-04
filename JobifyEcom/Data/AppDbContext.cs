using JobifyEcom.Models;
using Microsoft.EntityFrameworkCore;

namespace JobifyEcom.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<WorkerProfile> WorkerProfiles { get; set; }
    public DbSet<JobPost> JobPosts { get; set; }
}