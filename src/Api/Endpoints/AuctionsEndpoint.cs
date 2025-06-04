using System.Net.ServerSentEvents;
using Application.DTOs.Requests;
using Application.Extensions;
using Application.Interfaces;
using Application.Services;
using Domain.Entities;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;

namespace Api.Endpoints;

public static class AuctionsEndpoints
{
    public static RouteGroupBuilder MapAuctionsEndpoints(this WebApplication application)
    {
       var group = application.MapGroup("/auctions").WithTags("Auctions");
       
        group.MapPost("/start/{vehicleId:guid}", async (IAuctionService service, Guid vehicleId) =>
        {
            var startAuctionResult = await service.StartAuction(new VehicleId(vehicleId));
            return startAuctionResult.ToApiResult();
        });

        group.MapPost("/close/{vehicleId:guid}", async (IAuctionService service, Guid vehicleId) =>
        {
            var closeAuctionResult = await service.CloseAuction(new VehicleId(vehicleId));
            return closeAuctionResult.ToApiResult();
        });

        group.MapPost("/bid/{vehicleId:guid}", async (IAuctionService service, Guid vehicleId, BidRequest bid) =>
        {
            var bidResult = await service.PlaceBid(new VehicleId(vehicleId), bid.Bidder, bid.Amount.ToDomain());
            return bidResult.ToApiResult();
        });
        
        
        group.MapGet("/active/{vehicleId:guid}", (IAuctionMonitor monitor, Guid vehicleId, CancellationToken cancellationToken) =>
        {
            async IAsyncEnumerable<SseItem<Auction>> GetActiveAuctions()
            {
                await foreach (var auction in monitor.GetActiveAuctionsAsync(cancellationToken))
                {
                    if (auction.VehicleId == new VehicleId(vehicleId))
                    {
                        yield return new SseItem<Auction>(auction)
                        {
                            ReconnectionInterval = TimeSpan.FromMinutes(1)
                        };
                    }
                }
            }
            
            return TypedResults.ServerSentEvents(GetActiveAuctions());
            
        });

        return group;
    }
}