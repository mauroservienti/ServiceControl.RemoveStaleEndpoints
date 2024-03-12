using System.CommandLine;

namespace RemoveStaleEndpoints;

static class Commands
{
    static Option<string> UrlOption = new(["--url"], "The ServiceControl instance URL")
    {
        Arity = ArgumentArity.ExactlyOne,
    };

    static Option<TimeSpan> StaleDelayInMinutesOption = new(["--stale-delay"], () => TimeSpan.FromMinutes(5))
    {
        Description = "Stale delay, in minutes.",
        Arity = ArgumentArity.ZeroOrOne,
    };
    
    public static RootCommand Configure()
    {
        var root = new RootCommand("A tool to discover and purge ServiceControl and ServiceControl.Monitoring stale endpoints.");
        root.AddOption(UrlOption);
        root.AddOption(StaleDelayInMinutesOption);
        
        root.AddServiceControlCommands();


        // var purgeStaleMonitoredEndpointInstancesCommand = new Command("purge-stale-instances",
        //     "Purges ServiceControl.Monitoring endpoint instances that stopped stopped reporting metrics.");


        return root;
    }

    static void AddServiceControlCommands(this RootCommand rootCommand)
    {
        var reportServiceControlStaleEndpointsCommand = new Command("report-service-control-stale-endpoints", "Report ServiceControl stale endpoints");
        reportServiceControlStaleEndpointsCommand.SetHandler(async context =>
        {
            var serviceControlUrl = context.ParseResult.GetValueForOption(UrlOption);
            await ServiceControlApp.ReportServiceControlInactiveEndpoints(new Uri(serviceControlUrl!));
        });
        
        rootCommand.AddCommand(reportServiceControlStaleEndpointsCommand);
        
        var purgeServiceControlStaleEndpointsCommand = new Command("purge-service-control-stale-endpoints", "Purge ServiceControl stale endpoints");
        purgeServiceControlStaleEndpointsCommand.SetHandler(async context =>
        {
            var serviceControlUrl = context.ParseResult.GetValueForOption(UrlOption);
            var cutoff = context.ParseResult.GetValueForOption(StaleDelayInMinutesOption);
            await ServiceControlApp.PurgeServiceControlInactiveEndpoints(new Uri(serviceControlUrl!), cutoff);
        });
        
        rootCommand.AddCommand(purgeServiceControlStaleEndpointsCommand);
    }
    
    static void AddServiceControlMonitoringCommands(this RootCommand rootCommand)
    {
        var reportServiceControlMonitoringStaleInstancesCommand = new Command("report-service-control-monitoring-stale-instances", "Report ServiceControl.Monitoring stale endpoint instances");
        reportServiceControlMonitoringStaleInstancesCommand.SetHandler(async context =>
        {
            var serviceControlUrl = context.ParseResult.GetValueForOption(UrlOption);

        });
        
        rootCommand.AddCommand(reportServiceControlMonitoringStaleInstancesCommand);
        
        var purgeServiceControlStaleEndpointsCommand = new Command("purge-service-control-monitoring-stale-instances", "Purge ServiceControl.Monitoring stale endpoint instances");
        purgeServiceControlStaleEndpointsCommand.SetHandler(async context =>
        {
            var serviceControlUrl = context.ParseResult.GetValueForOption(UrlOption);
            var cutoff = context.ParseResult.GetValueForOption(StaleDelayInMinutesOption);
        });
        
        rootCommand.AddCommand(purgeServiceControlStaleEndpointsCommand);
    }
}