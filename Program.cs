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
    var certsDir = "/app/certs";
    var pfxPath = Path.Combine(certsDir, "cloudflare.pfx");

    // Debug: List all files in certs directory
    Console.WriteLine($"Looking for certificates in: {certsDir}");
    if (Directory.Exists(certsDir))
    {
        Console.WriteLine("Directory exists. Files found:");
        foreach (var file in Directory.GetFiles(certsDir))
        {
            Console.WriteLine($"  - {file}");
        }
    }
    else
    {
        Console.WriteLine("Directory does NOT exist!");
    }

    Console.WriteLine($"Checking for PFX at: {pfxPath}");
    Console.WriteLine($"PFX exists: {File.Exists(pfxPath)}");

    string? envCertPwd = Environment.GetEnvironmentVariable("CERT_PASSWORD");

    Console.WriteLine($"CERT_PASSWORD environment variable is {(string.IsNullOrEmpty(envCertPwd) ? "not set or empty" : "set")}");

    if (File.Exists(pfxPath))
    {
        Console.WriteLine("Loading HTTPS with PFX certificate...");
        serverOptions.ListenAnyIP(8080); // HTTP
        serverOptions.ListenAnyIP(8081, listenOptions => 
        {
            listenOptions.UseHttps(pfxPath, envCertPwd ?? "");
        });
        Console.WriteLine("HTTPS configured on port 8081");
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
app.MapGet("nonnullprice/{ticker}/{date}", RemoteEndPoint.GetNonNullPrice);
//


app.Run();
