using Serilog;

namespace Pulonia.Worker.Extensions;

public static class SerilogSetup
{
    public static IHostApplicationBuilder AddSerilogConfiguration(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services));

        return builder;
    }
}