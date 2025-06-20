using System.Net.ServerSentEvents;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Models.Responses;
using Domain.Entities.Vehicles;
using FluentValidation;

namespace Api.Endpoints;

public static class AuctionsEndpoints {
    public static WebApplication MapAuctionsEndpoints(this WebApplication application) {
        var group = application.MapGroup("/auctions").WithTags("Auctions");

        group.MapPost("/start/{vehicleId:guid}", async (IAuctionService service, Guid vehicleId) => {
            var startAuctionResult = await service.StartAuction(new VehicleId(vehicleId));
            return startAuctionResult.ToApiResult();
        });

        group.MapPost("/close/{vehicleId:guid}", async (IAuctionService service, Guid vehicleId) => {
            var closeAuctionResult = await service.CloseAuction(new VehicleId(vehicleId));
            return closeAuctionResult.ToApiResult();
        });

        group.MapPost("/bid/{vehicleId:guid}", async (IAuctionService service, IValidator<BidRequest> bidValidator, Guid vehicleId, BidRequest bidRequest) => {
            var validationResult = await bidValidator.ValidateAsync(bidRequest);
            if (!validationResult.IsValid) {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var bidResult = await service.PlaceBid(bidRequest, new VehicleId(vehicleId));
            return bidResult.ToApiResult();
        });

        group.MapGet("/active/{vehicleId:guid}", (IAuctionMonitor monitor, Guid vehicleId, CancellationToken cancellationToken) => {
            async IAsyncEnumerable<SseItem<AuctionMonitoringResponse>> GetActiveAuctions() {
                await foreach (var auction in monitor.GetActiveAuctionsAsync(cancellationToken)) {
                    if (auction.Vehicle.Id == vehicleId) {
                        yield return new SseItem<AuctionMonitoringResponse>(auction) {
                            ReconnectionInterval = TimeSpan.FromMinutes(1)
                        };
                    }
                }
            }

            return TypedResults.ServerSentEvents(GetActiveAuctions());

        });

        return application;
    }
}