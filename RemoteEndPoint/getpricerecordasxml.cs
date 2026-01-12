

using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;

public static partial class RemoteEndPoint
{
    public static async Task<IResult> GetPriceRecordAsXml(string ticker, DateOnly date, PriceContext db)
    {
        ticker = ticker.Trim().ToUpper();
        PriceRecord? priceRecord = await db.PriceRecords
            .Include(pr => pr.fundData)
            .FirstOrDefaultAsync(pr => pr.fundData.bloombergTicker == ticker && pr.date == date);

        if (priceRecord == null)
        {
            return Results.NotFound("No price records found.");
        }

        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(PriceRecord), new System.Xml.Serialization.XmlRootAttribute("PriceRecord"));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, priceRecord);
        var xmlResult = stringWriter.ToString();

        return Results.Content(xmlResult, "application/xml");
    }

    public static async Task<IResult> GetNonNullPriceAsXml(string ticker, string date, PriceContext db)
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

        PriceRecord? result = await db.PriceRecords
            .Include(pr => pr.fundData)
            .Where(pr => pr.fundData.bloombergTicker == ticker && pr.date == parsedDate)
            .FirstOrDefaultAsync();

        if (result is null)
        {
            return Results.NotFound();
        }

        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(PriceRecord), new System.Xml.Serialization.XmlRootAttribute("PriceRecord"));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, result);
        var xmlResult = stringWriter.ToString();

        return Results.Content(xmlResult, "application/xml");
    }
}