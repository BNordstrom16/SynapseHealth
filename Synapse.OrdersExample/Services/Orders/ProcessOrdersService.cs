using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Synapse.Models.Alerts;
using Synapse.Models.Orders;
using Synapse.Models.Updates;
using Synapse.Services.Alerts;
using Synapse.Services.Updates;

namespace Synapse.Services.Orders;

/// <summary>
/// I am a Service to Process Medical Equipment Orders
/// Typically would break the private methods into their own services to allow for thorough unit testing
/// In the interest of time, I kept the process all in one.
/// </summary>
/// <param name="logger">Logger instance</param>
/// <param name="ordersService">Order Service Refit Client</param>
/// <param name="alertsService">Alerts Service Refit Client</param>
/// <param name="updatesService">Updates Service Refit Client</param>
public class ProcessOrdersService(
    ILogger<ProcessOrdersService> logger,
    IOrdersService ordersService,
    IAlertsService alertsService,
    IUpdatesService updatesService)
    : IProcessOrdersService
{
    private const string DeliveredItem = "Delivered";
    
    public async Task ProcessOrdersAsync()
    {
        var medicalEquipmentOrders = await GetOrdersAsync();
        foreach (var order in medicalEquipmentOrders)
        {
            var updatedOrder = await ProcessOrderItemsAsync(order);
            await UpdateOrderAsync(updatedOrder);
        }
    }

    private async Task UpdateOrderAsync(OrdersResponse updatedOrder)
    {
        var updateOrderRequest = new UpdateOrderRequest(updatedOrder.OrderId, updatedOrder.Items);
        var updateOrderResponse = await updatesService.UpdateOrderAsync(updateOrderRequest);

        if (updateOrderResponse.IsSuccessStatusCode)
        {
            logger.LogInformation("Order Update Successful: OrderId {OrderId}", updatedOrder.OrderId);
        }
        else
        {
            logger.LogError("Order Update Failed: OrderId {OrderId}", updatedOrder.OrderId);
        }
    }

    private async Task<IEnumerable<OrdersResponse>> GetOrdersAsync()
    {
        var medicalEquipmentOrders = await ordersService.GetOrdersAsync();
        return medicalEquipmentOrders.IsSuccessful ? medicalEquipmentOrders.Content : new List<OrdersResponse>();
    }

    private async Task<OrdersResponse> ProcessOrderItemsAsync(OrdersResponse order)
    {
        foreach (var item in order.Items)
        {
            if (item.Status != DeliveredItem)
            {
                continue;
            }
            
            await SendAlertMessageAsync(item, order.OrderId);
            item.DeliveryNotification += 1;
        }

        return order;
    }

    private async Task SendAlertMessageAsync(OrderItem item, int orderId)
    {
        var alertsData = $"""
                         Alert for delivered item:
                             Order {orderId}, 
                             Item: {item.Description}, 
                             Delivery Notifications: {item.DeliveryNotification} 
                         """;
        var alertsRequest = new AlertsRequest(alertsData);
        var alertsResponse = await alertsService.SendAlertsAsync(alertsRequest);

        if (alertsResponse.IsSuccessStatusCode)
        {
            logger.LogInformation("Alert sent for delivered item: {ItemDescription}", item.Description);
        }
        else
        {
            logger.LogError("Alert failed for delivered item: {ItemDescription}", item.Description);
        }
    }
}