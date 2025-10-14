using Microsoft.EntityFrameworkCore;
using price_web_api.Models;

namespace price_web_api.Data
{
    public class PriceContext : DbContext
    {
        public PriceContext(DbContextOptions<PriceContext> options) : base(options) { }
        public DbSet<PriceRecord> PriceRecords { get; set; }
    }
}