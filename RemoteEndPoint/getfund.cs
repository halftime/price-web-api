
using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;

public static partial class RemoteEndPoint
{
    // Placeholder for remote endpoint methods

    public sealed record InvestmentsResponse(List<Fund> Funds, List<PreciousMetal> Metals);

    public static async Task<IResult> GetInvestmentByTicker(string ticker, PriceContext db)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }

        Investment? investment = await db.Investments.AsNoTracking()
            .FirstOrDefaultAsync(investment => investment.Symbol == ticker);

        return investment is not null ? Results.Ok(investment) : Results.NotFound();
    }

    public static async Task<IResult> GetFund(string ticker, PriceContext db)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }

        var fund = await db.FundInfos.FirstOrDefaultAsync(f => f.Symbol == ticker);
        return fund is not null ? Results.Ok(fund) : Results.NotFound();
    }

    public static async Task<IResult> GetFundAsXml(string ticker, PriceContext db)
    {
        ticker = ticker.Trim().ToUpper();
        if (string.IsNullOrWhiteSpace(ticker))
        {
            return Results.BadRequest("Ticker cannot be empty.");
        }

        var fund = await db.FundInfos.FirstOrDefaultAsync(f => f.Symbol == ticker);
        if (fund is null)
        {
            return Results.NotFound();
        }

        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Fund), new System.Xml.Serialization.XmlRootAttribute("FundInfo"));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, fund);
        var xmlResult = stringWriter.ToString();

        return Results.Content(xmlResult, "application/xml");
    }

    public static async Task<IResult> GetInvestments(PriceContext db)
    {
        var funds = await db.Investments.AsNoTracking()
            .OfType<Fund>()
            .OrderBy(investment => investment.Symbol)
            .ToListAsync();

        var metals = await db.PreciousMetals.AsNoTracking()
            .OrderBy(investment => investment.Symbol)
            .ToListAsync();

        return Results.Ok(new InvestmentsResponse(funds, metals));
    }
}