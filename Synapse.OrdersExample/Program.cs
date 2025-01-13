using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Refit;
using Serilog;
using Synapse.Services.Alerts;
using Synapse.Services.Background;
using Synapse.Services.Orders;
using Synapse.Services.Updates;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);
    
    // Clear existing loggers and add Serilog
    builder.Logging.ClearProviders();
    builder.Services.AddSerilog(Log.Logger);

    // Register Services
    builder
        .Services
        .AddRefitClient<IOrdersService>()
        .ConfigureHttpClient(opt => opt.BaseAddress = new Uri("https://orders-api.com"));
    builder
        .Services
        .AddRefitClient<IUpdatesService>()
        .ConfigureHttpClient(opt => opt.BaseAddress = new Uri("https://update-api.com"));
    builder
        .Services
        .AddRefitClient<IAlertsService>()
        .ConfigureHttpClient(opt => opt.BaseAddress = new Uri("https://alert-api.com"));
    builder
        .Services
        .AddSingleton<IProcessOrdersService, ProcessOrdersService>();
    builder
        .Services
        .AddHostedService<MedicalDeviceOrdersProcessingService>();
    
    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}