using Microsoft.EntityFrameworkCore;
using price_web_api.Models;

namespace price_web_api.Data;

public class PriceContext : DbContext
{
    public PriceContext(DbContextOptions<PriceContext> options) : base(options) { }
    public DbSet<PriceRecord> PriceRecords { get; set; } = null!;
    public DbSet<FundInfo> FundInfos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FundInfo>()
            .HasKey(f => f.fundId);

        modelBuilder.Entity<PriceRecord>()
            .HasKey(pr => new { pr.fundId, pr.date });

        modelBuilder.Entity<PriceRecord>()
            .HasOne(pr => pr.fundData)
            .WithMany(f => f.priceRecords);
    }
}
