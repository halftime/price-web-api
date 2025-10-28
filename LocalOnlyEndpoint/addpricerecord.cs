
using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;

using System.ComponentModel.DataAnnotations;

public static partial class LocalOnlyEndpoint
{
    public static async Task<IResult> AddPriceRecord(PriceRecord? priceRecord, PriceContext db)
    {
       if (priceRecord == null)
        {
            return Results.BadRequest("Request body is empty or could not be deserialized as PriceRecord.");
        }

        var validationContext = new ValidationContext(priceRecord);
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(priceRecord, validationContext, validationResults, validateAllProperties: true))
        {
            var errors = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
            return Results.BadRequest($"Validation failed: {errors}");
        }

        // Check if the associated FundInfo exists
        var fundExists = await db.FundInfos.AnyAsync(f => f.fundId == priceRecord.fundId);
        if (!fundExists)
        {
            return Results.Problem($"Fund with Id {priceRecord.fundId} does not exist.");
        }

        db.PriceRecords.Add(priceRecord);
        try
        {
            await db.SaveChangesAsync();
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

        Console.WriteLine($"Price record added: {priceRecord.Key} on {priceRecord.date}");

        return Results.Created($"/prices/{priceRecord.Key}/{priceRecord.date}", priceRecord);
    }
}