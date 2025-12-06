using Microsoft.EntityFrameworkCore;
using T_API.Shared.Models;

namespace PortfolioService.Data;

public class PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : DbContext(options)
{
    public DbSet<GoalItem> Goals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GoalItem>(entity =>
        {
            entity.ToTable("goal_items");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            entity.Property(e => e.Url).HasColumnName("url");
            entity.Property(e => e.Price).HasColumnName("price").HasPrecision(18, 2);
            entity.Property(e => e.PriceGap).HasColumnName("price_gap").HasPrecision(18, 2);
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(100);
            entity.Property(e => e.AddedAt).HasColumnName("added_at");
            entity.Property(e => e.CoolingUntil).HasColumnName("cooling_until");
            entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
            entity.Property(e => e.CompletedAt).HasColumnName("completed_at");
            entity.HasIndex(e => e.UserId);
        });
    }
}
