
using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;

public static partial class RemoteEndPoint
{
    // Placeholder for remote endpoint methods

    public static async Task<IResult> GetFund(string ticker, PriceContext db)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }

        var fund = await db.FundInfos.FirstOrDefaultAsync(f => f.bloombergTicker == ticker);
        return fund is not null ? Results.Ok(fund) : Results.NotFound();
    }
}