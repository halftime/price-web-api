using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;

namespace price_web_api.Services;

public interface IInvestmentCommandService
{
    Task<CreateInvestmentResult<Fund>> AddFundAsync(FundInfoRequest request, CancellationToken cancellationToken = default);
    Task<CreateInvestmentResult<PreciousMetal>> AddMetalAsync(PreciousMetalRequest request, CancellationToken cancellationToken = default);
}

public sealed record CreateInvestmentResult<T>(int StatusCode, T? Investment = default, string? Error = null, string? Location = null)
{
    public bool Succeeded => StatusCode is 201;
}

public sealed class InvestmentCommandService(PriceContext db) : IInvestmentCommandService
{
    public async Task<CreateInvestmentResult<Fund>> AddFundAsync(FundInfoRequest request, CancellationToken cancellationToken = default)
    {
        var ticker = string.IsNullOrWhiteSpace(request.bloombergTicker)
            ? request.symbol
            : request.bloombergTicker;

        if (string.IsNullOrWhiteSpace(ticker))
        {
            return new(400, Error: "bloombergTicker (or symbol) is required and cannot be empty.");
        }

        var canonicalName = string.IsNullOrWhiteSpace(request.fundName)
            ? request.Name
            : request.fundName;

        if (string.IsNullOrWhiteSpace(canonicalName))
        {
            return new(400, Error: "fundName (or Name) is required and cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.morningStarId))
        {
            return new(400, Error: "morningStarId is required and cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.iSIN))
        {
            return new(400, Error: "iSIN is required and cannot be empty.");
        }

        var normalizedTicker = ticker.Trim().ToUpper();
        var normalizedName = canonicalName.Trim();

        var exists = await db.Investments.OfType<Fund>()
            .AnyAsync(existingFund => existingFund.Symbol == normalizedTicker, cancellationToken)
            || await db.PreciousMetals
                .AnyAsync(existingMetal => existingMetal.Symbol == normalizedTicker, cancellationToken);
        if (exists)
        {
            return new(409, Error: $"An investment with Symbol '{normalizedTicker}' already exists.");
        }

        var fund = new Fund
        {
            Symbol = normalizedTicker,
            Name = normalizedName,
            MorningStarId = request.morningStarId.Trim(),
            ISIN = request.iSIN.Trim()
        };

        try
        {
            db.Investments.Add(fund);
            await db.SaveChangesAsync(cancellationToken);

            return new(201, fund, Location: $"/investment/{fund.Id}");
        }
        catch (DbUpdateException ex)
        {
            return new(400, Error: $"Failed to add fund due to database update error: {ex.GetBaseException().Message}");
        }
        catch (Exception ex)
        {
            return new(500, Error: $"Unexpected error while adding fund: {ex.Message}");
        }
    }

    public async Task<CreateInvestmentResult<PreciousMetal>> AddMetalAsync(PreciousMetalRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.symbol))
        {
            return new(400, Error: "symbol is required and cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(request.name))
        {
            return new(400, Error: "name is required and cannot be empty.");
        }

        var ticker = request.symbol;
        var canonicalName = request.name;

        var normalizedTicker = ticker.Trim().ToUpper();
        var normalizedName = canonicalName.Trim();

        var exists = await db.Investments.OfType<Fund>()
            .AnyAsync(existingFund => existingFund.Symbol == normalizedTicker, cancellationToken)
            || await db.PreciousMetals
                .AnyAsync(existingMetal => existingMetal.Symbol == normalizedTicker, cancellationToken);
        if (exists)
        {
            return new(409, Error: $"An investment with KeySymbol '{normalizedTicker}' already exists.");
        }

        var metal = new PreciousMetal
        {
            Symbol = normalizedTicker,
            Name = normalizedName,
        };

        try
        {
            db.Investments.Add(metal);
            await db.SaveChangesAsync(cancellationToken);

            return new(201, metal, Location: $"/investment/{metal.Id}");
        }
        catch (DbUpdateException ex)
        {
            return new(400, Error: $"Failed to add metal due to database update error: {ex.GetBaseException().Message}");
        }
        catch (Exception ex)
        {
            return new(500, Error: $"Unexpected error while adding metal: {ex.Message}");
        }
    }
}