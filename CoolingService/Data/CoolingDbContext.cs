using Microsoft.EntityFrameworkCore;
using T_API.Shared.Models;

namespace CoolingService.Data;

public class CoolingDbContext(DbContextOptions<CoolingDbContext> options) : DbContext(options)
{
    public DbSet<CoolingPeriodItem> CoolingPeriods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoolingPeriodItem>(entity =>
        {
            entity.ToTable("cooling_settings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PriceFrom).HasColumnName("price_from").HasPrecision(18, 2);
            entity.Property(e => e.PriceTo).HasColumnName("price_to").HasPrecision(18, 2);
            entity.Property(e => e.CoolingDays).HasColumnName("cooling_days");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.HasIndex(e => e.UserId);
        });
    }
}
