using price_web_api.Models;
using price_web_api.Services;

public static partial class RemoteEndPoint
{
    public static async Task<IResult> GetPriceRecordAsXml(string ticker, DateOnly date, IPriceRecordQueryService priceRecordQueryService)
    {
        ticker = ticker.Trim().ToUpper();
        MinimalPriceRec? priceRecord = await priceRecordQueryService.GetPriceRecordAsync(ticker, date);

        if (priceRecord == null)
        {
            return Results.NotFound("No price records found.");
        }

        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(MinimalPriceRec), new System.Xml.Serialization.XmlRootAttribute("PriceRecord"));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, priceRecord);
        var xmlResult = stringWriter.ToString();

        return Results.Content(xmlResult, "application/xml");
    }

    public static async Task<IResult> GetNonNullPriceAsXml(string ticker, string date, IPriceRecordQueryService priceRecordQueryService)
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

        MinimalPriceRec? result = await priceRecordQueryService.GetPriceRecordAsync(ticker, parsedDate);

        if (result is null)
        {
            return Results.NotFound();
        }

        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(MinimalPriceRec), new System.Xml.Serialization.XmlRootAttribute("PriceRecord"));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, result);
        var xmlResult = stringWriter.ToString();

        return Results.Content(xmlResult, "application/xml");
    }

    public static async Task<IResult> GetPriceRecordsByTickerAsXml(string ticker, IPriceRecordQueryService priceRecordQueryService)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }

        var priceRecords = await priceRecordQueryService.GetPriceRecordsByTickerAsync(ticker);

        if (priceRecords.Count == 0)
        {
            return Results.NotFound("No price records found.");
        }

        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(List<MinimalPriceRec>), new System.Xml.Serialization.XmlRootAttribute("PriceRecords"));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, priceRecords);
        var xmlResult = stringWriter.ToString();

        return Results.Content(xmlResult, "application/xml");
    }
}