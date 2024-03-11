using System.Text.Json;

namespace RemoveStaleEndpoints;

static class App
{
    const string serviceControlEndpointsRoute = "api/endpoints";
    const string serviceControlDeleteEndpointRoute = "api/endpoints/{endpointId}";
    
    // public static async Task PurgeServiceControlInactiveEndpoints(Uri serviceControlUri, TimeSpan cutoff)
    // {
    //     var client = new HttpClient()
    //     {
    //         BaseAddress = serviceControlUri
    //     };
    //
    //     var toDelete = await GetEndpointsToDelete(client, serviceControlEndpointsRoute, cutoff);
    //     await DeleteStaleEndpoints(toDelete, serviceControlDeleteEndpointRoute, client);
    // }
    //
    // static async Task DeleteStaleEndpoints(List<EndpointStatus> toDelete, string deleteEndpointRoute, HttpClient client)
    // {
    //     foreach (var endpointToDelete in toDelete)
    //     {
    //         Console.WriteLine($"Deleting endpoint {endpointToDelete.Name}.");
    //         var deleteUrl = deleteEndpointRoute.Replace("{endpointId}", endpointToDelete.Id);
    //         await client.DeleteAsync(deleteUrl);
    //         Console.WriteLine($"Endpoint {endpointToDelete.Name} deleted.");
    //     }
    // }

    // static async Task<List<EndpointStatus>> GetEndpointsToDelete(HttpClient client)
    // {
    //     var inactiveEndpoints = await GetInactiveEndpoints(client);
    //     var endpointsResponse = await client.GetAsync(endpointsRoute);
    //     var endpointsResponseString = await endpointsResponse.Content.ReadAsStringAsync();
    //     var endpoints = JsonSerializer.Deserialize<EndpointStatus[]>
    //     (
    //         endpointsResponseString,
    //         new JsonSerializerOptions()
    //         {
    //             PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    //         }
    //     );
    //
    //     var toDelete = new List<EndpointStatus>();
    //
    //     if (endpoints != null)
    //     {
    //         foreach (var endpoint in endpoints)
    //         {
    //             Console.WriteLine($"{endpoint.Name}/{endpoint.Id} -> {endpoint.HeartbeatInformation.ReportedStatus}");
    //             if (endpoint.HeartbeatInformation.ReportedStatus == "dead" &&
    //                 endpoint.HeartbeatInformation.LastReportAt < DateTime.Now.Subtract(cutoff))
    //             {
    //                 Console.WriteLine($"Going to delete endpoint {endpoint.Name}.");
    //                 toDelete.Add(endpoint);
    //             }
    //         }
    //     }
    //
    //
    //     return toDelete;
    // }
    
    static async Task<List<EndpointStatus>> GetInactiveEndpoints(HttpClient client)
    {
        var endpointsResponse = await client.GetAsync(serviceControlEndpointsRoute);
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
            Console.WriteLine($"\tEndpoint {endpoint.Name} is inactive, last reported at {endpoint.HeartbeatInformation.LastReportAt}.");
        }
    }
}