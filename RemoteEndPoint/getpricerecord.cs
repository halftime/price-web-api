using price_web_api.Models;
using price_web_api.Services;

public static partial class RemoteEndPoint
{
    // to confirm date is correctly deserialized from ISO format yyyy-MM-dd
    public static async Task<IResult> GetPriceRecord(string ticker, DateOnly date, IPriceRecordQueryService priceRecordQueryService)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }

        Console.WriteLine($"received request to get price record for ticker '{ticker}' on date '{date}'");

        var priceRecord = await priceRecordQueryService.GetPriceRecordAsync(ticker, date);

        return priceRecord is not null ? Results.Ok(priceRecord) : Results.NotFound();
    }

    public static async Task<IResult> GetPriceRecordsByTicker(string ticker, IPriceRecordQueryService priceRecordQueryService)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }

        Console.WriteLine($"received request to get all price records for ticker '{ticker}'");

        List<MinimalPriceRec> priceRecords = await priceRecordQueryService.GetPriceRecordsByTickerAsync(ticker);

        //List<MinimalPriceRec> priceRecords = await priceRecordQueryService.GetPriceRecordsByTickerAsync(ticker);

        return priceRecords.Any() ? Results.Ok(priceRecords) : Results.NotFound();
    }
}