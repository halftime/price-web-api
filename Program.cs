using Microsoft.EntityFrameworkCore;
using price_web_api.Data;
using price_web_api.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel for HTTPS
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    var certPath = "/app/certs/cloudflare-origin.pem";
    var keyPath = "/app/certs/cloudflare-origin.key";

    if (File.Exists(certPath) && File.Exists(keyPath))
    {
        serverOptions.ListenAnyIP(8080); // HTTP
        serverOptions.ListenAnyIP(8081, listenOptions =>
        {
            listenOptions.UseHttps(certPath, keyPath);
        });
    }
    else
    {
        // Fallback to HTTP only if certs not found
        serverOptions.ListenAnyIP(8080);
        Console.WriteLine("HTTPS certificates not found at /app/certs/, running HTTP only.");
    }
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<PriceContext>(options =>
    options.EnableSensitiveDataLogging()
           .UseSqlite(@"Data Source=/data/prices.db"));

var app = builder.Build();

// Configure HTTPS redirection and HSTS
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}
app.UseHttpsRedirection();

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
