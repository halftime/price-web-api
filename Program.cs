using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PriceContext>(options =>
    options.UseSqlite("Data Source=prices.db"));

var app = builder.Build();

// Ensure SQLite database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PriceContext>();
    db.Database.EnsureCreated();
}

app.MapPost("/addfund", async (FundInfo fund, PriceContext db) =>
{
    db.FundInfos.Add(fund);
    await db.SaveChangesAsync();
    return Results.Created($"/addfund/{fund.Id}", fund);
});


app.MapGet("/prices", async (HttpContext http, PriceContext db) =>
{
    //var prices = await db.PriceRecords.ToListAsync();
    return Results.Ok(new object[] { new { Id = 1, ProductName = "Sample Product", Price = 9.99M, Timestamp = DateTime.UtcNow } });
});

// Landing page GET /
app.MapGet("/test", () => Results.Content("<html><body><h1>test ok</h1></body></html>", "text/html"));

app.Run();
