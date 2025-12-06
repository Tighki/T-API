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
            entity.ToTable("user_preferences");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("user_id").HasMaxLength(255);
            entity.Property(e => e.MonthlySavings).HasColumnName("monthly_savings").HasPrecision(18, 2);
            entity.Property(e => e.CurrentSavings).HasColumnName("current_savings").HasPrecision(18, 2);
            entity.Property(e => e.ConsiderSavings).HasColumnName("consider_savings");
            entity.Property(e => e.NotificationFrequency).HasColumnName("notification_frequency").HasConversion<string>();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");
            entity.Property(e => e.BlacklistedCategories).HasColumnName("blacklisted_categories")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>()
                );
        });
    }
}
