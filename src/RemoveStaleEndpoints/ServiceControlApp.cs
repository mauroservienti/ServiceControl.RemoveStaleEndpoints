using System.Text.Json;

namespace RemoveStaleEndpoints;

static class ServiceControlApp
{
    public static async Task PurgeServiceControlInactiveEndpoints(Uri serviceControlUri, TimeSpan cutoff)
    {
        var client = new HttpClient()
        {
            BaseAddress = serviceControlUri
        };

        var inactiveEndpoints = await GetInactiveEndpoints(client);
        await DeleteStaleEndpoints(client, inactiveEndpoints, cutoff);
    }

    static async Task DeleteStaleEndpoints(HttpClient client, List<EndpointStatus> inactiveEndpoints, TimeSpan cutoff)
    {
        var endpointsToDelete = inactiveEndpoints.Where(status =>
            status.HeartbeatInformation.LastReportAt < DateTime.Now.Subtract(cutoff));
        foreach (var toDelete in endpointsToDelete)
        {
            Console.WriteLine($"Endpoint {toDelete.Name} reported the last heartbeat more than {cutoff} ago. And it's status is stale. Removing it...");
            var deleteUrl = $"api/endpoints/{toDelete.Id}";
            await client.DeleteAsync(deleteUrl);
            Console.WriteLine($"Endpoint {toDelete.Name} removed.");
        }
    }

    static async Task<List<EndpointStatus>> GetInactiveEndpoints(HttpClient client)
    {
        var endpointsResponse = await client.GetAsync("api/endpoints");
        var endpointsResponseString = await endpointsResponse.Content.ReadAsStringAsync();
        var endpoints = JsonSerializer.Deserialize<EndpointStatus[]>
        (
            endpointsResponseString,
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
            }
        );

        var inactive = endpoints?
            .Where(endpoint => endpoint.HeartbeatInformation.ReportedStatus == "dead")
            .ToList();

        return inactive ?? [];
    }

    public static async Task ReportServiceControlInactiveEndpoints(Uri serviceControlUri)
    {
        var client = new HttpClient()
        {
            BaseAddress = serviceControlUri
        };

        var inactiveEndpoints = await GetInactiveEndpoints(client);
        foreach (var endpoint in inactiveEndpoints)
        {
            Console.WriteLine($"{endpoint.Name}/{endpoint.Id} -> {endpoint.HeartbeatInformation.ReportedStatus}");
            Console.WriteLine(
                $"\tEndpoint {endpoint.Name} is inactive, last reported at {endpoint.HeartbeatInformation.LastReportAt}.");
        }
    }
}