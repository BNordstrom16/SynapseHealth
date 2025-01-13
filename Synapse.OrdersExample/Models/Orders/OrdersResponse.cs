namespace Synapse.Models.Orders;

/// <summary>
/// The Order and the Order's Items.
/// </summary>
public class OrdersResponse
{
    public int OrderId { get; set; }
    public IEnumerable<OrderItem> Items { get; set; }
}