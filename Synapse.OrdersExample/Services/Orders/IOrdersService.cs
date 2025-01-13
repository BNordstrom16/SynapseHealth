using Refit;
using Synapse.Models.Orders;

namespace Synapse.Services.Orders;

public interface IOrdersService
{
    /// <summary>
    /// I retrieve the Medical Equipment Orders
    /// </summary>
    /// <returns>A List of Orders and each Order's items</returns>
    [Get("/orders")]
    Task<ApiResponse<IEnumerable<OrdersResponse>>> GetOrdersAsync();
}