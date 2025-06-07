using System.Threading.Channels;
using Application.Extensions;
using Application.Interfaces;
using Application.Models.Requests;
using Application.Models.Responses;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AuctionService : IAuctionService {
    private readonly IVehicleRepository _vehicleRepo;
    private readonly IAuctionRepository _auctionRepo;
    private readonly ChannelWriter<AuctionMonitoringResponse> _auctionChannelWriter;
    private readonly ILogger<AuctionService> _logger;

    public AuctionService(
        IVehicleRepository vehicleRepo,
        IAuctionRepository auctionRepo,
        ChannelWriter<AuctionMonitoringResponse> auctionChannelWriter,
        ILogger<AuctionService> logger) {
        _vehicleRepo = vehicleRepo;
        _auctionRepo = auctionRepo;
        _auctionChannelWriter = auctionChannelWriter;
        _logger = logger;
    }

    //should be a start auction response not an auction
    public async Task<Result<StartAuctionResponse>> StartAuction(VehicleId vehicleId) {
        var vehicle = await _vehicleRepo.GetById(vehicleId);
        if (!VehicleExists(vehicle))
            return LogAndReturnFailure<StartAuctionResponse>("Attempted to start an auction for a non-existent vehicle", Problem.VehicleNotFound(vehicleId), vehicleId);

        if (await _auctionRepo.IsAuctionForVehicleActive(vehicleId))
            return LogAndReturnFailure<StartAuctionResponse>("Attempted to start an auction for a vehicle with an active auction", Problem.AuctionForVehicleAlreadyActive(vehicleId), vehicleId);

        _logger.LogInformation("Starting auction for vehicle: {VehicleId}", vehicleId);
        var createdAuction = new Auction(vehicleId);
        await _auctionRepo.Add(createdAuction);
        return Result<StartAuctionResponse>.Success(createdAuction.ToStartAuctionResponse(vehicle!));
    }

    public async Task<Result<AuctionClosedResponse>> CloseAuction(VehicleId vehicleId) {
        var auction = await _auctionRepo.GetActiveByVehicleId(vehicleId);
        if (auction is null)
            return LogAndReturnFailure<AuctionClosedResponse>("Attempted to close an auction for a vehicle without an active auction", Problem.AuctionForVehicleNotActive(vehicleId), vehicleId);

        _logger.LogInformation("Closing auction for vehicle: {VehicleId}", vehicleId);
        return await CloseAuction(auction);
    }

    private async Task<Result<AuctionClosedResponse>> CloseAuction(Auction auction) {
        if (!auction.IsActive)
            return Result<AuctionClosedResponse>.Failure(Problem.AuctionForVehicleNotActive(auction.VehicleId));

        var closedAuction = auction.Close();
        if (!closedAuction.IsSuccess)
            return Result<AuctionClosedResponse>.Failure(closedAuction.Problem!);

        await _vehicleRepo.Remove(auction.VehicleId);

        _logger.LogInformation("Auction closed for vehicle {VehicleId} with winner {Winner} and bid {WinningBid}", auction.VehicleId, closedAuction.Value!.Winner, closedAuction.Value.WinningBid);

        return Result<AuctionClosedResponse>.Success(new AuctionClosedResponse(
            auction.VehicleId.Id,
            closedAuction.Value.Winner,
            closedAuction.Value.WinningBid.ToDto()));
    }

    public async Task<Result<BidResponse>> PlaceBid(BidRequest bidRequest, VehicleId vehicleId) {
        var auction = await _auctionRepo.GetActiveByVehicleId(vehicleId);
        var vehicle = await _vehicleRepo.GetById(vehicleId);

        if (!VehicleExists(vehicle))
            return LogAndReturnFailure<BidResponse>("Attempted to place a bid on a non-existent vehicle", Problem.VehicleNotFound(vehicleId), vehicleId);

        if (auction is null) {
            return LogAndReturnFailure<BidResponse>("Attempted to place a bid on a vehicle without an active auction", Problem.AuctionForVehicleNotActive(vehicleId), vehicleId);
        }
        if (!auction.IsActive) {
            return LogAndReturnFailure<BidResponse>("Attempted to place a bid on a closed auction", Problem.Closed(auction.Id), vehicleId);
        }

        _logger.LogInformation("Placing bid for vehicle {VehicleId} by {Bidder}: {Money}", vehicleId, bidRequest.Bidder, bidRequest.Amount);

        var bid = bidRequest.RequestToDomain();
        var bidResult = auction.PlaceBid(bid, vehicle!);
        if (!bidResult.IsSuccess) {
            return LogAndReturnFailure<BidResponse>("Failed to place bid", bidResult.Problem!, vehicleId, bidRequest.Bidder, bidRequest.Amount.ToDomain());
        }

        await _auctionChannelWriter.WriteAsync(auction.ToMonitoringResponse(vehicle!, bid));
        return Result<BidResponse>.Success(new BidResponse(vehicle!.ToDto(), bid.Bidder, bid.Value.ToDto()));
    }

    private static bool VehicleExists(Vehicle? vehicle) => vehicle is not null;

    private Result<T> LogAndReturnFailure<T>(string message, Problem problem, VehicleId vehicleId, string? bidder = null, Money? money = null) {
        if (bidder is not null && money is not null)
            _logger.LogWarning("{Message} for vehicle {VehicleId} by {Bidder}: {Money}", message, vehicleId, bidder, money);
        else
            _logger.LogWarning("{Message}: {VehicleId}", message, vehicleId);

        return Result<T>.Failure(problem);
    }
}