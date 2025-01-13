namespace Synapse.Models.Orders;

/// <summary>
/// The item that is attached to an Order
/// </summary>
public class OrderItem
{
    public int OrderItemId { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public int DeliveryNotification { get; set; } = 0;
}