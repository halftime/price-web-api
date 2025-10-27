using System.Net;

public class LocalNetworkOnlyFilter : IEndpointFilter
{
    private static readonly HashSet<string> AllowedIps = new()
    {
        "127.0.0.1",          // localhost IPv4
        "::1",                // localhost IPv6
        "172.17.0.1",         // Docker bridge gateway
        // add more if needed: "192.168.1.42", "10.0.0.5", etc.
    };

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

        // Allow only if the string representation matches
        if (AllowedIps.Contains(remoteIp.ToString()))
        {
            Console.WriteLine($"localnetworkfilter \t IP: {remoteIp} is local, OK");
            return await next(context);
        }

        Console.WriteLine($"localnetworkfilter \t IP: {remoteIp} is NOT local, DENIED");
        return Results.StatusCode(StatusCodes.Status403Forbidden);
    }
}
