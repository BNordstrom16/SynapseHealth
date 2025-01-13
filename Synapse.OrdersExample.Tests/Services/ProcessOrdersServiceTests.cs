using System.Net;
using AutoFixture;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Refit;
using Synapse.Models.Alerts;
using Synapse.Models.Orders;
using Synapse.Models.Updates;
using Synapse.Services.Alerts;
using Synapse.Services.Orders;
using Synapse.Services.Updates;

namespace Synapse.OrdersExample.Tests.Services;

public class ProcessOrdersServiceTests
{
    private readonly Fixture _fixture = new();
    private readonly ILogger<ProcessOrdersService> _logger;
    
    public ProcessOrdersServiceTests()
    {
        var loggerFactory = LoggerFactory.Create(builder => 
            builder.AddConsole()
        );
        _logger = loggerFactory.CreateLogger<ProcessOrdersService>();
    }

    [Fact]
    public async Task ProcessOrdersService_ProcessOrders_ProcessesOrders()
    {
        // Arrange
        var logger = A.Fake<ILogger<ProcessOrdersService>>();
        
        var ordersService = A.Fake<IOrdersService>();
        var generatedOrdersResponse = GenerateOrders(2);
        A.CallTo(() => ordersService.GetOrdersAsync()).Returns(generatedOrdersResponse);

        var alertsService = A.Fake<IAlertsService>();
        var generatedAlertResponse = CreateHttpResponseMessage(HttpStatusCode.OK);
        A.CallTo(() => alertsService.SendAlertsAsync(An<AlertsRequest>._)).Returns(generatedAlertResponse);

        var updateService = A.Fake<IUpdatesService>();
        var generatedUpdateResponse = CreateHttpResponseMessage(HttpStatusCode.OK);
        A.CallTo(() => updateService.UpdateOrderAsync(An<UpdateOrderRequest>._)).Returns(generatedUpdateResponse);

        var processOrdersService = new ProcessOrdersService(_logger, ordersService, alertsService, updateService);

        // Act
        await processOrdersService.ProcessOrdersAsync();
        
        // Assert
        A.CallTo(() => ordersService.GetOrdersAsync()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ProcessOrdersService_ProcessOrders_GetOrdersAsync_ThrowsError()
    {
        // Arrange
        
        var alertsService = A.Fake<IAlertsService>();
        var updateService = A.Fake<IUpdatesService>();
        
        var ordersService = A.Fake<IOrdersService>();
        var apiException = await ApiException.Create(
            new HttpRequestMessage(HttpMethod.Get, ""),
            HttpMethod.Get,
            CreateHttpResponseMessage(HttpStatusCode.BadRequest),
            new RefitSettings());
        A.CallTo(() => ordersService.GetOrdersAsync()).ThrowsAsync(apiException);

        var processOrdersService = new ProcessOrdersService(_logger, ordersService, alertsService, updateService);
        
        // Act & Assert
        await Assert.ThrowsAsync<ApiException>(() => processOrdersService.ProcessOrdersAsync());
    }

    private ApiResponse<IEnumerable<OrdersResponse>> GenerateOrders(int count)
    {
        var random = new Random();
        var orderItems = _fixture
            .Build<OrderItem>()
            .WithAutoProperties()
            .With(x => x.Status, "Delivered")
            .CreateMany(random.Next(2, 8));
        
        var orders = _fixture
            .Build<OrdersResponse>()
            .With(x => x.Items, orderItems)
            .CreateMany(count);

        var httpResponseMessage = CreateHttpResponseMessage(HttpStatusCode.OK);

        return new ApiResponse<IEnumerable<OrdersResponse>>(httpResponseMessage, orders, new RefitSettings());
    }

    private static HttpResponseMessage CreateHttpResponseMessage(HttpStatusCode statusCode)
    {
        return new HttpResponseMessage
        {
            StatusCode = statusCode,
            Content = new StringContent("")
        };
    }
}