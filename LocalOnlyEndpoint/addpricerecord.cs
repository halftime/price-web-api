
using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;

using System.ComponentModel.DataAnnotations;

public static partial class LocalOnlyEndpoint
{
    public static async Task<IResult> AddPriceRecord(MinimalPriceRec? priceRecord, PriceContext db)
    {
        if (priceRecord == null)
        {
            return Results.BadRequest("Request body is empty or could not be deserialized as MinimalPriceRec.");
        }
        if (priceRecord.price <= 0)
        {
            return Results.BadRequest("Price must be greater than zero.");
        }

        // Require symbol in the payload
        if (string.IsNullOrWhiteSpace(priceRecord.Symbol))
        {
            return Results.BadRequest("Symbol is required in the price record.");
        }

        // Find investment by symbol (case-insensitive)
        var normalizedSymbol = priceRecord.Symbol.Trim().ToUpper();
        var investment = await db.Investments.FirstOrDefaultAsync(i => i.Symbol.ToUpper() == normalizedSymbol);
        if (investment == null)
        {
            return Results.BadRequest($"No investment found with symbol '{priceRecord.Symbol}'.");
        }

        // Check for existing price record by investment id and date
        MinimalPriceRec? existingRecord = await db.PriceRecords.FirstOrDefaultAsync(pr => pr.Symbol.ToLower() == investment.Symbol.ToLower() && pr.date == priceRecord.date);
        if (existingRecord != null)
        {
            // Conflict: record already exists, do update and return
            existingRecord.price = priceRecord.price;
            await db.SaveChangesAsync();
            return Results.Conflict(existingRecord);
        }

        // Set the correct InvestmentId
        db.PriceRecords.Add(priceRecord);
        try
        {
            int savedchanges = await db.SaveChangesAsync();
            Console.WriteLine($"SaveChangesAsync returned {savedchanges} when adding price record for symbol={priceRecord.Symbol} on date={priceRecord.date:yyyy-MM-dd}");
        }
        catch (DbUpdateConcurrencyException ex) // duplicate 
        {
            return Results.Conflict($"A concurrency conflict occurred while adding the price record: {ex.GetBaseException().Message}");
        }
        catch (DbUpdateException ex) // duplicate key 
        {
            return Results.Conflict($"A database update error occurred while adding the price record: {ex.GetBaseException().Message}");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Unexpected error while adding price record: {ex.Message}");
        }

        Console.WriteLine($"Price record added: symbol={priceRecord.Symbol}, date={priceRecord.date:yyyy-MM-dd}");
        // Return Created with the correct URI (no named route needed)
        return Results.Created($"/pricerecord/{investment.Symbol}/{priceRecord.date:yyyy-MM-dd}", priceRecord);
    }
}