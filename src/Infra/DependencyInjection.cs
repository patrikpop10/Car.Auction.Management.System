using Microsoft.Extensions.DependencyInjection;

namespace Infra;

public static class DependencyInjection {
    public static IServiceCollection AddInfraServices(this IServiceCollection services) {
        // Register repositories
        services.AddSingleton<Domain.Repositories.IVehicleRepository, Repositories.InMemoryVehicleRepository>();
        services.AddSingleton<Domain.Repositories.IAuctionRepository, Repositories.InMemoryAuctionRepository>();

        return services;
    }
}