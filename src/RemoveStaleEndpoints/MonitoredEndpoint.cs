using System.Text.Json.Serialization;

namespace RemoveStaleEndpoints;

public class MonitoredEndpoint
{
    public string Name { get; set; }
    public bool IsStale { get; set; }

    [JsonIgnore]
    public MonitoredEndpointInstance[] StaleInstances { get; set; } = [];
    public int DisconnectedCount { get; set; }
    public int ConnectedCount { get; set; }
}

public class MonitoredEndpointInstance
{
    public string Id { get; set; }
    public bool IsStale { get; set; }
}