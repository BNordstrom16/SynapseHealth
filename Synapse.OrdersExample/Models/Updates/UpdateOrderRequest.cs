using Synapse.Models.Orders;

namespace Synapse.Models.Updates;

public class UpdateOrderRequest(int orderId, IEnumerable<OrderItem> items)
{
    public int OrderId { get; set; } = orderId;
    public IEnumerable<OrderItem> Items { get; set; } = items;
}