using Pulonia.Application;
using Pulonia.Infrastructure;
using Pulonia.Worker.BackgroundServices;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddApplicationServices();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
