using Pulonia.Application;
using Pulonia.Infrastructure;
using Pulonia.Worker.BackgroundServices;
using Pulonia.Worker.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Pulonia Worker Service...");

    var builder = Host.CreateApplicationBuilder(args);

    // serilog extension
    builder.AddSerilogConfiguration();

    // register services from application and infrastructure layers
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddApplicationServices();

    // ping configuration
    builder.Services.Configure<Pulonia.Application.Common.Configurations.PingConfiguration>(
        builder.Configuration.GetSection("PingSettings"));

    // Register worker background service
    builder.Services.AddHostedService<Worker>();

    var host = builder.Build();
    host.Run();
}
catch (Exception e)
{
    Log.Fatal("[ERROR] Failed to start server: {err}", e);
}
finally
{
    Log.CloseAndFlush();
}