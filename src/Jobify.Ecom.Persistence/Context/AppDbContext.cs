using Jobify.Ecom.Domain.Entities.Users;
using Jobify.Ecom.Domain.Entities.UserSessions;
using Microsoft.EntityFrameworkCore;

namespace Jobify.Ecom.Persistence.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
}
