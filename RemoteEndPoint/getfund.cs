
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

    public static async Task<IResult> GetFundAsXml(string ticker, PriceContext db)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }

        var fund = await db.FundInfos.FirstOrDefaultAsync(f => f.bloombergTicker == ticker);
        if (fund is null)
        {
            return Results.NotFound();
        }

        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(FundInfo), new System.Xml.Serialization.XmlRootAttribute("FundInfo"));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, fund);
        var xmlResult = stringWriter.ToString();

        return Results.Content(xmlResult, "application/xml");
    }
}