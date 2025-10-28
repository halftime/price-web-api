using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PriceContext>(options =>
    options.EnableSensitiveDataLogging()
           .UseSqlite("Data Source=prices.db"));

var app = builder.Build();

// Ensure SQLite database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PriceContext>();
    db.Database.EnsureCreated();
}

// Post methods for creating records 
// Local network only
app.MapPost("addpricerecord", LocalOnlyEndpoint.AddPriceRecord).AddEndpointFilter<LocalNetworkOnlyFilter>();
app.MapPost("addfund", LocalOnlyEndpoint.AddFund).AddEndpointFilter<LocalNetworkOnlyFilter>();
// ***


// Get methods for retrieving records
app.MapGet("funds/{ticker}", RemoteEndPoint.GetFund);
app.MapGet("prices/{ticker}", RemoteEndPoint.GetPriceRecordsByTicker);
app.MapGet("pricerecord/{ticker}/{date}", RemoteEndPoint.GetPriceRecord);
app.MapGet("pricerecord.xml/{ticker}/{date}", RemoteEndPoint.GetPriceRecordAsXml);
//


app.Run();
