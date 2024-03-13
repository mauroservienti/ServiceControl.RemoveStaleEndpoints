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

        var staleEndpoints = endpoints?
            .Where(endpoint => endpoint.IsStale)
            .ToList() ?? [];

        foreach (var staleEndpoint in staleEndpoints)
        {
            var staleEndpointDetailsResponse = await client.GetAsync($"monitored-endpoints/{staleEndpoint.Name}");
            var document =
                await JsonDocument.ParseAsync(await staleEndpointDetailsResponse.Content.ReadAsStreamAsync());
            var instancesProperty = document.RootElement.GetProperty("instances");
            var monitoredEndpointInstances = instancesProperty.Deserialize<MonitoredEndpointInstance[]>(
                new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            staleEndpoint.StaleInstances =
                monitoredEndpointInstances?.Where(instance => instance.IsStale).ToArray() ?? [];
        }

        return staleEndpoints;
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
            foreach (var instance in endpoint.StaleInstances)
            {
                Console.WriteLine($"\tInstance ID {instance.Id} is stale.");
            }
        }
    }
}