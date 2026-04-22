using Microsoft.EntityFrameworkCore;
using price_web_api.Models;

namespace price_web_api.Data;

public class PriceContext : DbContext
{
    public PriceContext(DbContextOptions<PriceContext> options) : base(options) { }
    public DbSet<Investment> Investments { get; set; } = null!;
    public DbSet<MinimalPriceRec> PriceRecords { get; set; } = null!;
    public DbSet<Fund> FundInfos { get; set; } = null!;
    public DbSet<PreciousMetal> PreciousMetals { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Investment>()
            .HasKey(i => i.Id);

        modelBuilder.Entity<Investment>()
            .HasIndex(i => i.Symbol)
            .IsUnique();

        modelBuilder.Entity<MinimalPriceRec>()
            .HasKey(pr => new { pr.Symbol, pr.date });

    }
}
