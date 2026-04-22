namespace price_web_api.Models;

public class FundInfoRequest
{
    public int? Id { get; set; }
    public string? symbol { get; set; }
    public string? Name { get; set; }
    public string? bloombergTicker { get; set; }
    public string? fundName { get; set; }
    public string? morningStarId { get; set; }
    public string? iSIN { get; set; }
}