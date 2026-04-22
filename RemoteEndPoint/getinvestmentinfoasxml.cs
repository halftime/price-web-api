using price_web_api.Models;
using price_web_api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Xml.Serialization;
using price_web_api.Data;

public static partial class RemoteEndPoint
{
    public static async Task<IResult> GetInvestmentInfoAsXml(string ticker, IPriceRecordQueryService priceRecordQueryService, PriceContext db)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }


        // Try to find a Fund first (TPH: use db.Set<Fund>())
        var fund = await db.Set<Fund>().FirstOrDefaultAsync(f => f.Symbol.ToUpper() == ticker);
        if (fund != null)
        {
            var xml = SerializeToXml(fund);
            return Results.Content(xml, "application/xml");
        }

        // Try to find a PreciousMetal (TPH: use db.Set<PreciousMetal>())
        var metal = await db.Set<PreciousMetal>().FirstOrDefaultAsync(m => m.Symbol.ToUpper() == ticker);
        if (metal != null)
        {
            var xml = SerializeToXml(metal);
            return Results.Content(xml, "application/xml");
        }

        // Fallback: try to find any Investment
        var investment = await db.Investments.FirstOrDefaultAsync(i => i.Symbol.ToUpper() == ticker);
        if (investment != null)
        {
            var xml = SerializeToXml(investment);
            return Results.Content(xml, "application/xml");
        }

        return Results.NotFound();
    }

    private static string SerializeToXml<T>(T obj)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter, obj);
        return stringWriter.ToString();
    }
}