using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace price_web_api.Data;

public class PriceContextFactory : IDesignTimeDbContextFactory<PriceContext>
{
    public PriceContext CreateDbContext(string[] args)
    {
        var dbDirectory = "/data/pricewebapi";
        var dbPath = System.IO.Path.Combine(dbDirectory, "prices.db");

        var optionsBuilder = new DbContextOptionsBuilder<PriceContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new PriceContext(optionsBuilder.Options);
    }
}
