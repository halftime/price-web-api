

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
}