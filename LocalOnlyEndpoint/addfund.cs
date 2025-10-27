using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;

public static partial class LocalOnlyEndpoint
{
    public static async Task<IResult> AddFund(FundInfo? fund, PriceContext db)
    {

        if (fund == null)
        {
            Console.WriteLine("received request to add fund: null");
            return Results.BadRequest("Request body is empty or could not be deserialized as FundInfo.");
        }

        Console.WriteLine("received request to add fund:", fund.bloombergTicker);

        if (string.IsNullOrWhiteSpace(fund.bloombergTicker))
        {
            return Results.BadRequest("BloombergTicker is required and cannot be empty.");
        }

        // Prevent duplicate tickers
        var exists = await db.FundInfos.AnyAsync(f => f.bloombergTicker == fund.bloombergTicker);
        if (exists)
        {
            return Results.Conflict($"A fund with BloombergTicker '{fund.bloombergTicker}' already exists.");
        }

        try
        {
            db.FundInfos.Add(fund);
            await db.SaveChangesAsync();
            return Results.Created($"/funds/{Uri.EscapeDataString(fund.bloombergTicker)}", fund);
        }
        catch (DbUpdateException ex)
        {
            return Results.BadRequest($"Failed to add fund due to database update error: {ex.GetBaseException().Message}");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Unexpected error while adding fund: {ex.Message}");
        }
    }
}