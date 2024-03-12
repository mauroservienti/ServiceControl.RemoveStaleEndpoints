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
        
        ConfigureServiceControlCommands(root);


        // var purgeStaleMonitoredEndpointInstancesCommand = new Command("purge-stale-instances",
        //     "Purges ServiceControl.Monitoring endpoint instances that stopped stopped reporting metrics.");


        return root;
    }

    static void ConfigureServiceControlCommands(RootCommand rootCommand)
    {
        var reportServiceControlStaleEndpointsCommand = new Command("report-service-control-stale-endpoints", "Report ServiceControl stale endpoints");
        reportServiceControlStaleEndpointsCommand.SetHandler(async context =>
        {
            var serviceControlUrl = context.ParseResult.GetValueForOption(UrlOption);
            await App.ReportServiceControlInactiveEndpoints(new Uri(serviceControlUrl!));
        });
        
        rootCommand.AddCommand(reportServiceControlStaleEndpointsCommand);
        
        var purgeServiceControlStaleEndpointsCommand = new Command("purge-service-control-stale-endpoints", "Purge ServiceControl stale endpoints");
        purgeServiceControlStaleEndpointsCommand.SetHandler(async context =>
        {
            
        });
        
        rootCommand.AddCommand(purgeServiceControlStaleEndpointsCommand);
    }

    static void ConfigureReportServiceControlInactiveEndpointsCommand(RootCommand root)
    {
        var cmd = new Command("report-inactive-endpoints",
            "Lists ServiceControl decommissioned endpoints that stopped sending heartbeats.");
        
        var serviceControlUrlArg = new Argument<string>("sc-url",
            getDefaultValue: () => Path.Combine(Path.GetTempPath(), "http://localhost:33333/"),
            description: "The ServiceControl primary instance URL (without the '/api')");
        cmd.AddArgument(serviceControlUrlArg);
        
        cmd.SetHandler(async context =>
        {
            var serviceControlUrl = context.ParseResult.GetValueForArgument(serviceControlUrlArg);
            await App.ReportServiceControlInactiveEndpoints(new Uri(serviceControlUrl));
        });
    }

    // static void ConfigurePurgeServiceControlInactiveEndpointsCommand(RootCommand root)
    // {
    //     var cmd = new Command("purge-inactive-endpoints",
    //         "Purges ServiceControl decommissioned endpoints that stopped sending heartbeats.");
    //
    //     var serviceControlUrlArg = new Argument<string>("sc-url",
    //         getDefaultValue: () => Path.Combine(Path.GetTempPath(), "http://localhost:33333/"),
    //         description: "The ServiceControl primary instance URL (without the '/api')");
    //     cmd.AddArgument(serviceControlUrlArg);
    //
    //     var cutoffArg = new Argument<TimeSpan>("cutoff",
    //         getDefaultValue: () => TimeSpan.FromMinutes(5),
    //         description: "The time since endpoints reported the last heartbeat (TimeSpan).");
    //     cmd.AddArgument(cutoffArg);
    //
    //     cmd.SetHandler(async context =>
    //     {
    //         var serviceControlUrl = context.ParseResult.GetValueForArgument(serviceControlUrlArg);
    //         var cutoff = context.ParseResult.GetValueForArgument(cutoffArg);
    //         await App.PurgeServiceControlInactiveEndpoints(new Uri(serviceControlUrl), cutoff);
    //     });
    //
    //     root.AddCommand(cmd);
    // }
}