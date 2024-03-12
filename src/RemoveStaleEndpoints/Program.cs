using System.CommandLine;
using RemoveStaleEndpoints;

var root = Commands.Configure();
return await root.InvokeAsync(args);

// // ServiceControl.Monitoring API details
// //var serviceControlMonitoringUrl = "http://localhost:33633/";
// var monitoredEndpointsRoute = ;
// var deleteMonitoredEndpointInstanceRoute = "monitored-instance/{endpointName}/{instanceId}";