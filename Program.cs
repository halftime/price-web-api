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

// Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.MapOpenApi();
// }
app.MapGet("/prices", async (HttpContext http, PriceContext db) =>
{
    //var prices = await db.PriceRecords.ToListAsync();
    return Results.Ok(new object[] { new { Id = 1, ProductName = "Sample Product", Price = 9.99M, Timestamp = DateTime.UtcNow } });
});

app.MapPost("/prices", async (PriceRecord record, PriceContext db) =>
{
    db.PriceRecords.Add(record);
    await db.SaveChangesAsync();
    return Results.Created($"/prices/{record.Id}", record);
});

// Landing page GET /
app.MapGet("/", () => Results.Content("<html><body><h1>Hello World</h1></body></html>", "text/html"));

app.Run();
