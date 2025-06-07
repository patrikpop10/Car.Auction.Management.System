using Domain.Repositories;
using Infra.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infra;

public static class DependencyInjection {
    public static IServiceCollection AddInfraServices(this IServiceCollection services) {
        // Register repositories
        services.AddSingleton<IVehicleRepository, InMemoryVehicleRepository>();
        services.AddSingleton<IAuctionRepository, InMemoryAuctionRepository>();

        return services;
    }
}