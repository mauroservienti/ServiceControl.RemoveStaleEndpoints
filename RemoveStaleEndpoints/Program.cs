using System.Text.Json;
using System.CommandLine;

Option<string> serviceControlUrlOption = new(["--sc-url"], () => "http://localhost:33333/")
{
    Description = "ServiceControl URL",
    Arity = ArgumentArity.ZeroOrOne,
};

Option<string> serviceControlMonitoringUrlOption = new(["--scm-url"], () => "http://localhost:33633/")
{
    Description = "ServiceControl.Monitoring URL",
    Arity = ArgumentArity.ZeroOrOne,
};

Option<TimeSpan> staleDelayInMinutes = new(["--stale-delay"], () => TimeSpan.FromMinutes(5))
{
    Description = "Stale delay, in minutes.",
    Arity = ArgumentArity.ZeroOrOne,
};

var root = new RootCommand("A tool that emulates a multi-endpoint NServiceBus system to smoke test ServiceControl instances");
root.AddOption(serviceControlUrlOption);
root.AddOption(serviceControlMonitoringUrlOption);
root.AddOption(staleDelayInMinutes);

root.SetHandler(async context =>
{
    var serviceControlUrl = context.ParseResult.GetValueForOption(serviceControlUrlOption);
});

return await root.InvokeAsync(args);

// //var staleDelay = TimeSpan.FromMinutes(1);
//
// // ServiceControl API details
// //var serviceControlUrl = "http://localhost:33333/";
// var endpointsRoute = "api/endpoints";
// var deleteEndpointRoute = "api/endpoints/{endpointId}";
//
// // ServiceControl.Monitoring API details
// //var serviceControlMonitoringUrl = "http://localhost:33633/";
// var monitoredEndpointsRoute = "monitored-endpoints";
// var deleteMonitoredEndpointInstanceRoute = "monitored-instance/{endpointName}/{instanceId}";
//
//
// var toDelete = await GetEndpointsToDelete(client, endpointsRoute, staleDelay);
//
// foreach (var endpointToDelete in toDelete)
// {
//     var deleteUrl = deleteEndpointRoute.Replace("{endpointId}", endpointToDelete.Id);
//     await client.DeleteAsync(deleteUrl);
// }
//
// static async Task<List<EndpointStatus>> GetEndpointsToDelete(HttpClient client, string endpointsRoute, TimeSpan staleDelay)
// {
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
//                 endpoint.HeartbeatInformation.LastReportAt < DateTime.Now.Subtract(staleDelay))
//             {
//                 Console.WriteLine($"Going to delete endpoint {endpoint.Name}");
//                 toDelete.Add(endpoint);
//             }
//         }
//     }
//
//
//     return toDelete;
// }