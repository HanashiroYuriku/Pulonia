using Pulonia.Application.Common.Interfaces.Authentication;
using Pulonia.Application.Common.Interfaces.Repositories;
using Pulonia.Infrastructure.Authentication;
using Pulonia.Infrastructure.Data;
using Pulonia.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pulonia.Application.Common.Interfaces.ExternalServices;
using Pulonia.Infrastructure.ExternalServices;

namespace Pulonia.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // === Pomelo MySQL
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        // register Pomelo
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        // register Password Hash
        services.AddScoped<IPasswordHasher, BycryptPasswordHasher>();
        // register JWT Settings
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        // register JWT Generator
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        // register Database Transaction Manager
        services.AddScoped<IDbTransactionManager, DbTransactionManager>();

        // register user repository
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<INetworkService, NetworkService>();

        services.AddScoped<IStoreRepository, StoreRepository>();
        services.AddScoped<IPingLogRepository, PingLogRepository>();
        services.AddScoped<IDailySlaRepository, DailySlaRepository>();
        // register other repositories here

        // ..

        return services;
    }
}