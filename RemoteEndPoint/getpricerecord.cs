
using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;

public static partial class RemoteEndPoint
{
    // to confirm date is correctly deserialized from ISO format yyyy-MM-dd
    public static async Task<IResult> GetPriceRecord(string ticker, DateOnly date, PriceContext db)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }

        Console.WriteLine($"received request to get price record for ticker '{ticker}' on date '{date}'");

        var priceRecord = await db.PriceRecords
            .Include(pr => pr.fundData)
            .FirstOrDefaultAsync(pr => pr.fundData.bloombergTicker == ticker && pr.date == date);

        return priceRecord is not null ? Results.Ok(priceRecord) : Results.NotFound();
    }

    public static async Task<IResult> GetPriceRecordsByTicker(string ticker, PriceContext db)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }

        Console.WriteLine($"received request to get all price records for ticker '{ticker}'");

        List<PriceRecord>? priceRecords = await db.PriceRecords
            .Include(pr => pr.fundData)
            .Where(pr => pr.fundData.bloombergTicker == ticker)
            .OrderBy(pr => pr.date) // older to newer
            .ToListAsync();

        return priceRecords?.Any() == true ? Results.Ok(priceRecords) : Results.NotFound();
    }
}