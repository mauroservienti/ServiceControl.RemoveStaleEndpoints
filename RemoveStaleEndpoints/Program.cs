using System.Text.Json;

var staleDelay = TimeSpan.FromMinutes(1);

// ServiceControl API details
var serviceControlUrl = "http://localhost:33333/";
var endpointsRoute = "api/endpoints";
var deleteEndpointRoute = "api/endpoints/{endpointId}";

// ServiceControl.Monitoring API details
var serviceControlMonitoringUrl = "http://localhost:33633/";
var monitoredEndpointsRoute = "monitored-endpoints";
var deleteMonitoredEndpointInstanceRoute = "monitored-instance/{endpointName}/{instanceId}";

var client = new HttpClient();
client.BaseAddress = new Uri(serviceControlUrl);

var toDelete = await GetEndpointsToDelete(client, endpointsRoute);

foreach (var endpointToDelete in toDelete)
{
    var deleteUrl = deleteEndpointRoute.Replace("{endpointId}", endpointToDelete.Id);
    await client.DeleteAsync(deleteUrl);
}

static async Task<List<EndpointStatus>> GetEndpointsToDelete(HttpClient client, string endpointsRoute)
{
    var endpointsResponse = await client.GetAsync(endpointsRoute);
    var endpointsResponseString = await endpointsResponse.Content.ReadAsStringAsync();
    var endpoints = JsonSerializer.Deserialize<EndpointStatus[]>
    (
        endpointsResponseString,
        new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        }
    );
    
    var toDelete = new List<EndpointStatus>();

    if (endpoints != null)
    {
        foreach (var endpoint in endpoints)
        {
            Console.WriteLine($"{endpoint.Name}/{endpoint.Id} -> {endpoint.HeartbeatInformation.ReportedStatus}");
            if (endpoint.HeartbeatInformation.ReportedStatus == "dead" &&
                endpoint.HeartbeatInformation.LastReportAt < DateTime.Now.Subtract(staleDelay))
            {
                Console.WriteLine($"Going to delete endpoint {endpoint.Name}");
                toDelete.Add(endpoint);
            }
        }
    }


    return toDelete;
}