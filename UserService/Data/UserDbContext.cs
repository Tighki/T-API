using Microsoft.EntityFrameworkCore;
using T_API.Shared.Models;

namespace UserService.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<UserPreferences> UserPreferences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserPreferences>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasMaxLength(100);
            entity.Property(e => e.MonthlySavings).HasPrecision(18, 2);
            entity.Property(e => e.CurrentSavings).HasPrecision(18, 2);
            entity.Property(e => e.NotificationFrequency).HasConversion<string>();
            entity.Property(e => e.BlacklistedCategories)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );
        });
    }
}
