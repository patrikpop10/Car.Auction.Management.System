using System.Threading.Channels;
using Application.Interfaces;
using Application.Models.Dtos;
using Application.Models.Requests;
using Application.Models.Responses;
using Application.Services;
using Application.Validators.DtosValidators;
using Application.Validators.RequestValidators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection  AddApplicationServices(this IServiceCollection services)
    {
        
        // Create a channel for auction monitoring
        var channel = Channel.CreateUnbounded<AuctionMonitoringResponse>(new UnboundedChannelOptions 
        {
            SingleReader = true,
            SingleWriter = false
        });

        services.AddSingleton(channel.Reader);
        services.AddSingleton(channel.Writer);

        // Register services
        services.AddScoped<IAuctionService, AuctionService>();
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddSingleton<IAuctionMonitor, AuctionMonitor>();

        return services;
    }
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<BidRequest>, BidRequestValidator>();
        services.AddScoped<IValidator<VehicleRequest>, VehicleRequestValidator>();
        services.AddScoped<IValidator<MoneyDto>, MoneyDtoValidator>();
        services.AddScoped<IValidator<VehicleDto>, VehicleDtoValidator>();

        return services;
    }
}