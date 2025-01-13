namespace Synapse.Models.Alerts;

public class AlertsRequest(string message)
{
    public string Message { get; set; } = message;
}