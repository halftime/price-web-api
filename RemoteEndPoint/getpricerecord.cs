
using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;

public static partial class RemoteEndPoint
{
    public static async Task<IResult> GetPriceRecord(string ticker, DateOnly date, PriceContext db)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }

        var priceRecord = await db.PriceRecords
            .Include(pr => pr.fundData)
            .FirstOrDefaultAsync(pr => pr.fundData.bloombergTicker == ticker && pr.date == date);

        return priceRecord is not null ? Results.Ok(priceRecord) : Results.NotFound();
    }
}