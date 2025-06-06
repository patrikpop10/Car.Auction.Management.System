using System.Net.ServerSentEvents;
using Application.DTOs.Requests;
using Application.Extensions;
using Application.Interfaces;
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
            var bidResult = await service.PlaceBid(bid, new VehicleId(vehicleId));
            return bidResult.ToApiResult();
        });
        
        
        group.MapGet("/active/{vehicleId:guid}", (IAuctionMonitor monitor, Guid vehicleId, CancellationToken cancellationToken) =>
        {
            async IAsyncEnumerable<SseItem<AuctionMonitoringResponse>> GetActiveAuctions()
            {
                await foreach (var auction in monitor.GetActiveAuctionsAsync(cancellationToken))
                {
                    if (auction.Auction.VehicleId == vehicleId)
                    {
                        yield return new SseItem<AuctionMonitoringResponse>(auction)
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