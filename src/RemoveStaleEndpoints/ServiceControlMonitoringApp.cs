using System.Text.Json;

namespace RemoveStaleEndpoints;

public class ServiceControlMonitoringApp
{
    static async Task<List<MonitoredEndpoint>> GetStaleInstances(HttpClient client)
    {
        var endpointsResponse = await client.GetAsync("monitored-endpoints");
        var endpointsResponseString = await endpointsResponse.Content.ReadAsStringAsync();
        var endpoints = JsonSerializer.Deserialize<MonitoredEndpoint[]>
        (
            endpointsResponseString,
            new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }
        );

        var stale = endpoints?
            .Where(endpoint => endpoint.IsStale)
            .ToList();

        return stale ?? [];
    }
    
    public static async Task ReportStaleInstances(Uri serviceControlUri)
    {
        var client = new HttpClient()
        {
            BaseAddress = serviceControlUri
        };

        var inactiveEndpoints = await GetStaleInstances(client);
        foreach (var endpoint in inactiveEndpoints)
        {
            Console.WriteLine($"{endpoint.Name} is stale with {endpoint.DisconnectedCount} disconnected instances.");
        }
    }
}