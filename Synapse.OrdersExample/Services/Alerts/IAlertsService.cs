using Refit;
using Synapse.Models.Alerts;

namespace Synapse.Services.Alerts;

public interface IAlertsService
{
    /// <summary>
    /// I send alerts for the delivered Order Items
    /// </summary>
    /// <param name="request">Message for the item of the order that has been delivered</param>
    /// <returns></returns>
    [Post("/alerts")]
    Task<HttpResponseMessage> SendAlertsAsync([Body] AlertsRequest request);
}