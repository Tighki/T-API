using Microsoft.EntityFrameworkCore;
using T_API.Shared.Models;

namespace PortfolioService.Data;

public class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : base(options) { }

    public DbSet<GoalItem> Goals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GoalItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserId).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.PriceGap).HasPrecision(18, 2);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.HasIndex(e => e.UserId);
        });
    }
}
