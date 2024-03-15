# RemoveStaleEndpoints

The `RemoveStaleEndpoints` command line tool removes inactive endpoints from ServiceControl primary instances and endpoint instances from ServiceControl Monitoring instances.

## Reporting

The `report-service-control-stale-endpoints` and `report-service-control-monitoring-stale-instances` commands allow listing inactive endpoints and endpoint instances.

The complete command syntax for reporting inactive ServiceControl endpoints is:

```shell
servicecontrol-remove-stale-endpoints report-service-control-stale-endpoints --url http://localhost:33333/
```

To report inactive ServiceControl Monitoring endpoint instances, use:

```shell
servicecontrol-remove-stale-endpoints report-service-control-monitoring-stale-instances --url http://localhost:33633
```

## Purging

The `purge-service-control-stale-endpoints` and `purge-service-control-monitoring-stale-instances` commands allow the purging of inactive endpoints and endpoint instances.

To purge ServiceControl inactive endpoints:

```shell
servicecontrol-remove-stale-endpoints purge-service-control-stale-endpoints --url http://localhost:33333/ --cutoff 00:00:10
```

The `cutoff` argument (Optional. It defaults to 15 minutes) determines how long endpoints should have been stale before being removed.

To purge ServiceControl Monitoring inactive endpoint instances:

```shell
servicecontrol-remove-stale-endpoints purge-service-control-monitoring-stale-instances --url http://localhost:33633
```

## Installing

> With .NET 8 installed

```shell
dotnet tool install -g ServiceControl.RemoveStaleEndpoints --add-source https://f.feedz.io/mauroservienti/pre-releases/nuget/index.json
```

## Updating

```shell
dotnet tool update -g ServiceControl.RemoveStaleEndpoints --add-source https://f.feedz.io/mauroservienti/pre-releases/nuget/index.json
```
