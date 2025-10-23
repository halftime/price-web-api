using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PriceContext>(options =>
    options.EnableSensitiveDataLogging()
           .UseSqlite("Data Source=prices.db"));

var app = builder.Build();

// Ensure SQLite database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PriceContext>();
    db.Database.EnsureCreated();
}

// Post methods for creating records 

app.MapPost("addpricerecord", async (PriceRecord? priceRecord, PriceContext db) =>
{
    Console.WriteLine("received request to add price record, " + priceRecord?.ToString());

    if (priceRecord == null)
    {
        return Results.BadRequest("Request body is empty or could not be deserialized as PriceRecord.");
    }
    // Check if the associated FundInfo exists
    var fundExists = await db.FundInfos.AnyAsync(f => f.fundId == priceRecord.fundId);
    if (!fundExists)
    {
        return Results.BadRequest($"Fund with Id {priceRecord.fundId} does not exist.");
    }

    db.PriceRecords.Add(priceRecord);
    await db.SaveChangesAsync();

    Console.WriteLine($"Price record added: {priceRecord.Key} on {priceRecord.date}");
    return Results.Created($"/prices/{priceRecord.Key}/{priceRecord.date}", priceRecord);
});

/// Funds, tested ok
app.MapPost("addfund", async (FundInfo fund, PriceContext db) =>
{
    Console.WriteLine("received request to add fund:", fund.bloombergTicker);
    if (fund == null)
    {
        return Results.BadRequest("Request body is empty or could not be deserialized as FundInfo.");
    }

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
});

// Get methods for retrieving records
app.MapGet("funds/{ticker}", async (string ticker, PriceContext db) =>
{
    ticker = ticker.Trim().ToUpper();
    if (string.IsNullOrWhiteSpace(ticker))
    {
        return Results.BadRequest("Ticker cannot be empty.");
    }

    var fund = await db.FundInfos.FirstOrDefaultAsync(f => f.bloombergTicker == ticker);
    return fund is not null ? Results.Ok(fund) : Results.NotFound();
});

app.MapGet("prices/{ticker}/{date}", async (string ticker, DateOnly date, PriceContext db) =>
{
    if (string.IsNullOrWhiteSpace(ticker))
    {
        return Results.BadRequest("Ticker cannot be empty.");
    }

    var priceRecord = await db.PriceRecords
        .Include(pr => pr.fundData)
        .FirstOrDefaultAsync(pr => pr.fundData.bloombergTicker == ticker && pr.date == date);

    return priceRecord is not null ? Results.Ok(priceRecord) : Results.NotFound();
});

app.Run();
