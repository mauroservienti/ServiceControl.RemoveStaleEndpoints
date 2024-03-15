using System.Text.Json;

namespace ServiceControl.RemoveStaleEndpoints;

static class ServiceControlApp
{
    public static async Task PurgeInactiveEndpoints(Uri serviceControlUri, TimeSpan cutoff)
    {
        var client = new HttpClient()
        {
            BaseAddress = serviceControlUri
        };

        var inactiveEndpoints = await GetInactiveEndpoints(client);
        if (inactiveEndpoints.Count == 0)
        {
            Console.WriteLine("There are no stale endpoints");
            return;
        }

        await DeleteStaleEndpoints(client, inactiveEndpoints, cutoff);
    }

    static async Task DeleteStaleEndpoints(HttpClient client, List<EndpointStatus> inactiveEndpoints, TimeSpan cutoff)
    {
        var endpointsToDelete = inactiveEndpoints
            .Where(status => status.HeartbeatInformation.LastReportAt < DateTime.UtcNow.Subtract(cutoff))
            .ToList();
        if (endpointsToDelete.Count == 0)
        {
            Console.WriteLine($"There are no stale endpoints older than the supplied cutoff ({cutoff})");
            return;
        }

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

    public static async Task ReportInactiveEndpoints(Uri serviceControlUri)
    {
        var client = new HttpClient()
        {
            BaseAddress = serviceControlUri
        };

        var inactiveEndpoints = await GetInactiveEndpoints(client);
        if (inactiveEndpoints.Count == 0)
        {
            Console.WriteLine("There are no stale endpoints");
            return;
        }
        
        foreach (var endpoint in inactiveEndpoints)
        {
            Console.WriteLine($"{endpoint.Name} stopped reporting heartbeats");
            Console.WriteLine(
                $"\tEndpoint {endpoint.Name} is inactive, last reported at {endpoint.HeartbeatInformation.LastReportAt}.");
        }
    }
}