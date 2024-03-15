using System.CommandLine;
using ServiceControl.RemoveStaleEndpoints;

var root = Commands.Configure();
return await root.InvokeAsync(args);