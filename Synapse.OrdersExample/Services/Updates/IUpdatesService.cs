using Refit;
using Synapse.Models.Updates;

namespace Synapse.Services.Updates;

public interface IUpdatesService
{
    /// <summary>
    /// I update the Medical Equipment Order to be processed
    /// </summary>
    /// <param name="request">The items and order id for the order</param>
    /// <returns></returns>
    [Post("/update")]
    Task<HttpResponseMessage> UpdateOrderAsync([Body] UpdateOrderRequest request);
}