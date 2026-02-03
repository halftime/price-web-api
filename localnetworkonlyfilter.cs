using System.Net;

public class LocalNetworkOnlyFilter : IEndpointFilter
{
    private static readonly HashSet<string> AllowedSubnets =
    [
        "192.168.0.",         // 192.168.0.x range
        "192.168.1.",         // 192.168.1.x range (optional)
        "172.17.0.",        // 172.17.x.x range (Docker default)
        "172.18.0.",        // 172.18.x.x range (Docker alternative)
        "192.168.129."  // new range for local net
    ];

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var remoteIp = context.HttpContext.Connection.RemoteIpAddress;

        if (remoteIp == null)
        {
            Console.WriteLine("localnetworkfilter \t Remote IP is null");
            return Results.StatusCode(StatusCodes.Status400BadRequest);
        }

        // Convert IPv4-mapped IPv6 (e.g. ::ffff:172.17.0.1) to plain IPv4
        if (remoteIp.IsIPv4MappedToIPv6)
            remoteIp = remoteIp.MapToIPv4();

        var ipString = remoteIp.ToString();

        // Check if IP matches any allowed subnet prefix
        if (AllowedSubnets.Any(ipString.StartsWith))
        {
            Console.WriteLine($"localnetworkfilter \t IP: {remoteIp} is in allowed subnet, OK");
            return await next(context);
        }

        Console.WriteLine($"localnetworkfilter \t IP: {remoteIp} is NOT local, DENIED");
        return Results.StatusCode(StatusCodes.Status403Forbidden);
    }
}
