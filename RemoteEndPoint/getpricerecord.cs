
using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;

public static partial class RemoteEndPoint
{
    public static async Task<IResult> GetNonNullPrice(string ticker, string date, PriceContext db)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }
        if (!DateOnly.TryParse(date, out DateOnly parsedDate))
        {
            return Results.BadRequest("Invalid date format. Please use ISO format yyyy-MM-dd.");
        }

        Console.WriteLine($"received request to get non null price record for ticker '{ticker}' on date '{parsedDate}'");

        PriceRecord? price_close = await db.PriceRecords
            .Include(pr => pr.fundData)
            .Where(pr => pr.fundData.bloombergTicker == ticker && pr.date == parsedDate && pr.close > 0)
            .OrderByDescending(pr => pr.close)
            .FirstOrDefaultAsync();

        PriceRecord? price_open = await db.PriceRecords
            .Include(pr => pr.fundData)
            .Where(pr => pr.fundData.bloombergTicker == ticker && pr.date == parsedDate && pr.open > 0)
            .OrderByDescending(pr => pr.open)
            .FirstOrDefaultAsync();

        PriceRecord? price_nav = await db.PriceRecords
            .Include(pr => pr.fundData)
            .Where(pr => pr.fundData.bloombergTicker == ticker && pr.date == parsedDate && pr.nav > 0)
            .OrderByDescending(pr => pr.nav)
            .FirstOrDefaultAsync();

        PriceRecord? result = price_close ?? price_open ?? price_nav;

        return result is not null ? Results.Ok(result) : Results.NotFound();
    }


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