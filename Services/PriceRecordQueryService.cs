using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;

namespace price_web_api.Services;

public interface IPriceRecordQueryService
{
    Task<MinimalPriceRec?> GetPriceRecordAsync(string ticker, DateOnly date, CancellationToken cancellationToken = default);
    Task<List<MinimalPriceRec>> GetPriceRecordsByTickerAsync(string ticker, CancellationToken cancellationToken = default);
}

public sealed class PriceRecordQueryService(PriceContext db) : IPriceRecordQueryService
{
    public Task<MinimalPriceRec?> GetPriceRecordAsync(string ticker, DateOnly date, CancellationToken cancellationToken = default)
    {
        var normalizedTicker = ticker.Trim().ToUpper();
            int investmentId = db.Investments.AsNoTracking()
            .Where(i => i.Symbol == normalizedTicker)
            .Select(i => i.Id)
            .FirstOrDefault();

        return db.PriceRecords.AsNoTracking()
            .Where(pr => pr.symbol.ToUpper() == normalizedTicker && pr.date == date)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<MinimalPriceRec>> GetPriceRecordsByTickerAsync(string ticker, CancellationToken cancellationToken = default)
    {
        var normalizedTicker = ticker.Trim().ToUpper();

        // Investment? existingInvstm = await db.Investments.AsNoTracking()
        //     .Where(i => i.Symbol == normalizedTicker)
        //     .FirstOrDefaultAsync(cancellationToken);

        return await db.PriceRecords.AsNoTracking().Where(pr => pr.symbol.ToUpper() == normalizedTicker).ToListAsync(cancellationToken);
    }
}