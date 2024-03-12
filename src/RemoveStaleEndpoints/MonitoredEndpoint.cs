namespace RemoveStaleEndpoints;

public class MonitoredEndpoint
{
    public string Name { get; set; }
    public bool IsStale { get; set; }
    public string[] EndpointInstanceIds { get; set; } = [];
    public int DisconnectedCount { get; set; }
    public int ConnectedCount { get; set; }
}