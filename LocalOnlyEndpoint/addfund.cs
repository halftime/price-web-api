using price_web_api.Models;
using price_web_api.Services;

public static partial class LocalOnlyEndpoint
{
    public static async Task<IResult> AddMetal(PreciousMetalRequest? investment, IInvestmentCommandService investmentCommandService)
    {
        if (investment == null)
        {
            Console.WriteLine("received request to add metal investment: null");
            return Results.BadRequest("Request body is empty or could not be deserialized as PreciousMetal.");
        }

        var result = await investmentCommandService.AddMetalAsync(investment);
        return ToCreateResult(result);
    }

    public static async Task<IResult> AddFund(FundInfoRequest? investment, IInvestmentCommandService investmentCommandService)
    {

        if (investment == null)
        {
            Console.WriteLine("received request to add fund investment: null");
            return Results.BadRequest("Request body is empty or could not be deserialized as FundInfo.");
        }

        var result = await investmentCommandService.AddFundAsync(investment);
        return ToCreateResult(result);
    }

    private static IResult ToCreateResult<T>(CreateInvestmentResult<T> result)
    {
        if (result.Succeeded && result.Investment is not null && !string.IsNullOrWhiteSpace(result.Location))
        {
            return Results.Created(result.Location, result.Investment);
        }

        return result.StatusCode switch
        {
            409 => Results.Conflict(result.Error),
            400 => Results.BadRequest(result.Error),
            _ => Results.Problem(result.Error)
        };
    }
}