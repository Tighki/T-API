using Microsoft.EntityFrameworkCore;
using T_API.Shared.Models;

namespace CoolingService.Data;

public class CoolingDbContext : DbContext
{
    public CoolingDbContext(DbContextOptions<CoolingDbContext> options) : base(options) { }

    public DbSet<CoolingPeriodItem> CoolingPeriods { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CoolingPeriodItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasMaxLength(100).IsRequired();
            entity.Property(e => e.PriceFrom).HasPrecision(18, 2);
            entity.Property(e => e.PriceTo).HasPrecision(18, 2);
            entity.HasIndex(e => e.UserId);
        });
    }
}
