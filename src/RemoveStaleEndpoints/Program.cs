using System.CommandLine;
using RemoveStaleEndpoints;

var root = Commands.Configure();
return await root.InvokeAsync(args);