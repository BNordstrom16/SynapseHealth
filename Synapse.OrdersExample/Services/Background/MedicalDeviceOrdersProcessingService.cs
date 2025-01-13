using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Synapse.Services.Orders;

namespace Synapse.Services.Background;

/// <summary>
/// I am a background service that will be processing the Medical Device Orders
/// </summary>
/// <param name="logger">Logger instance for class</param>
/// <param name="processOrdersService">Processes orders on a 10 second timer</param>
public class MedicalDeviceOrdersProcessingService(
        ILogger<MedicalDeviceOrdersProcessingService> logger,
        IProcessOrdersService processOrdersService
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("MedicalDeviceOrdersProcessingService Starting {Time}", DateTime.UtcNow);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await processOrdersService.ProcessOrdersAsync();
                
                // Process requests every 10 seconds
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }    
            catch (OperationCanceledException)
            {
                logger.LogInformation("MedicalDeviceOrdersProcessingService Stopping {Time}", DateTime.UtcNow);
                break;
            }
            catch (Exception ex)
            {
                logger.LogError("MedicalDeviceOrdersProcessingService Error {Error}", ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}