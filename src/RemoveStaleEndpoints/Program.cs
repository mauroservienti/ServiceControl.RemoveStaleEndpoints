using System.Text.Json;
using System.CommandLine;
using RemoveStaleEndpoints;

// Option<string> serviceControlMonitoringUrlOption = new(["--scm-url"], () => "http://localhost:33633/")
// {
//     Description = "ServiceControl.Monitoring URL",
//     Arity = ArgumentArity.ZeroOrOne,
// };
//
// Option<TimeSpan> staleDelayInMinutes = new(["--stale-delay"], () => TimeSpan.FromMinutes(5))
// {
//     Description = "Stale delay, in minutes.",
//     Arity = ArgumentArity.ZeroOrOne,
// };

var root = Commands.Configure();
return await root.InvokeAsync(args);

// //var staleDelay = TimeSpan.FromMinutes(1);
//
// // ServiceControl API details
// //var serviceControlUrl = "http://localhost:33333/";

//
// // ServiceControl.Monitoring API details
// //var serviceControlMonitoringUrl = "http://localhost:33633/";
// var monitoredEndpointsRoute = "monitored-endpoints";
// var deleteMonitoredEndpointInstanceRoute = "monitored-instance/{endpointName}/{instanceId}";
//
//

//
