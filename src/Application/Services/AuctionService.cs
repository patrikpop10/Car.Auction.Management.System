using System.Threading.Channels;
using Application.DTOs;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Extensions;
using Application.Interfaces;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;
using Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AuctionService : IAuctionService
{
    private readonly IVehicleRepository _vehicleRepo;
    private readonly IAuctionRepository _auctionRepo;
    private readonly ChannelWriter<Auction> _auctionChannelWriter;
    private readonly ILogger<AuctionService> _logger;

    public AuctionService(
        IVehicleRepository vehicleRepo,
        IAuctionRepository auctionRepo,
        ChannelWriter<Auction> auctionChannelWriter,
        ILogger<AuctionService> logger)
    {
        _vehicleRepo = vehicleRepo;
        _auctionRepo = auctionRepo;
        _auctionChannelWriter = auctionChannelWriter;
        _logger = logger;
    }
    
    //should be a start auction response not an auction
    public async Task<Result<StartAuctionResponse>> StartAuction(VehicleId vehicleId)
    {
        var vehicle = await _vehicleRepo.GetById(vehicleId);
        if (!VehicleExists(vehicle))
            return LogAndReturnFailure<StartAuctionResponse>("Attempted to start an auction for a non-existent vehicle", Problem.VehicleNotFound(vehicleId), vehicleId);

        if (await _auctionRepo.IsAuctionActive(vehicleId))
            return LogAndReturnFailure<StartAuctionResponse>("Attempted to start an auction for a vehicle with an active auction", Problem.AuctionAlreadyActive(vehicleId), vehicleId);

        _logger.LogInformation("Starting auction for vehicle: {VehicleId}", vehicleId);
        var auction = new Auction(vehicleId);
        await _auctionRepo.Add(auction);
        return Result<StartAuctionResponse>.Success(auction.ToStartAuctionResponse(vehicle!));
    }

    public async Task<Result<AuctionClosedResponse>> CloseAuction(VehicleId vehicleId)
    {
        var auction = await _auctionRepo.GetActiveByVehicleId(vehicleId);
        if (auction is null)
            return LogAndReturnFailure<AuctionClosedResponse>("Attempted to close an auction for a vehicle without an active auction", Problem.AuctionNotActive(vehicleId), vehicleId);

        _logger.LogInformation("Closing auction for vehicle: {VehicleId}", vehicleId);
        var auctionClosed = CloseAuction(auction);
        return auctionClosed;

    }

    private Result<AuctionClosedResponse> CloseAuction(Auction auction)
    {
        if (!auction.IsActive)
            return Result<AuctionClosedResponse>.Failure(Problem.AuctionNotActive(auction.VehicleId));

        var closedAuction =  auction.Close();
        if (!closedAuction.IsSuccess)
            return Result<AuctionClosedResponse>.Failure(closedAuction.Problem!);

        _logger.LogInformation("Auction closed for vehicle {VehicleId} with winner {Winner} and bid {WinningBid}", auction.VehicleId, closedAuction.Value!.Winner, closedAuction.Value.WinningBid);

        return Result<AuctionClosedResponse>.Success(new AuctionClosedResponse(
            auction.VehicleId.Id,
            closedAuction.Value.Winner,
            closedAuction.Value.WinningBid.ToDto()));
    }

    public async Task<Result<BidResponse>> PlaceBid(BidDto bidDto)
    {
        var auction = await _auctionRepo.GetActiveByVehicleId(bidDto.Id);
        var vehicle = await _vehicleRepo.GetById(bidDto.Id);

        if (!VehicleExists(vehicle))
            return LogAndReturnFailure<BidResponse>("Attempted to place a bid on a non-existent vehicle", Problem.VehicleNotFound(bidDto.Id), bidDto.Id);

        if (auction is null)
            return LogAndReturnFailure<BidResponse>("Attempted to place a bid on a vehicle without an active auction", Problem.AuctionNotActive(bidDto.Id), bidDto.Id);
        

        _logger.LogInformation("Placing bid for vehicle {VehicleId} by {Bidder}: {Money}", bidDto.Id, bidDto.Bidder, bidDto.Amount);

        var bid = bidDto.ToDomain();
        var bidResult = auction.PlaceBid(bid, vehicle!);
        if (!bidResult.IsSuccess)
        {
            return LogAndReturnFailure<BidResponse>("Failed to place bid", bidResult.Problem!, bidDto.Id, bidDto.Bidder, bidDto.Amount.ToDomain());
        }

        await _auctionChannelWriter.WriteAsync(auction);
        return Result<BidResponse>.Success(new BidResponse(vehicle!.ToDto(), bid.Bidder, bid.Value.ToDto()));
    }

    private static bool VehicleExists(Vehicle? vehicle) => vehicle is not null;
    

    private Result<T> LogAndReturnFailure<T>(string message, Problem problem, VehicleId vehicleId, string? bidder = null, Money? money = null)
    {
        if (bidder is not null && money is not null)
            _logger.LogWarning("{Message} for vehicle {VehicleId} by {Bidder}: {Money}", message, vehicleId, bidder, money);
        else
            _logger.LogWarning("{Message}: {VehicleId}", message, vehicleId);

        return Result<T>.Failure(problem);
    }
}