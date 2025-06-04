using System.Threading.Channels;
using Application.DTOs.Responses;
using Application.Extensions;
using Application.Interfaces;
using Domain;
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

    public async Task<Result<AuctionClosed>> CloseAuction(VehicleId vehicleId)
    {
        var auction = await _auctionRepo.GetActiveByVehicleId(vehicleId);
        if (auction is null)
            return LogAndReturnFailure<AuctionClosed>("Attempted to close an auction for a vehicle without an active auction", Problem.AuctionNotActive(vehicleId), vehicleId);

        _logger.LogInformation("Closing auction for vehicle: {VehicleId}", vehicleId);
        var closed = await _auctionRepo.CloseAuction(auction);
        return closed ? auction.Close() : Result<AuctionClosed>.Failure(Problem.AuctionNotFound(vehicleId));
    }

    public async Task<Result<BidResult>> PlaceBid(VehicleId vehicleId, string bidder, Money money)
    {
        var auction = await _auctionRepo.GetActiveByVehicleId(vehicleId);
        var vehicle = await _vehicleRepo.GetById(vehicleId);

        if (!VehicleExists(vehicle))
            return LogAndReturnFailure<BidResult>("Attempted to place a bid on a non-existent vehicle", Problem.VehicleNotFound(vehicleId), vehicleId);

        if (auction is null)
            return LogAndReturnFailure<BidResult>("Attempted to place a bid on a vehicle without an active auction", Problem.AuctionNotActive(vehicleId), vehicleId);

        if (!BidIsValid(money, auction, vehicle!))
            return LogAndReturnFailure<BidResult>("Invalid bid attempt", Problem.InvalidBidAmount(), vehicleId, bidder, money);

        _logger.LogInformation("Placing bid for vehicle {VehicleId} by {Bidder}: {Money}", vehicleId, bidder, money);
        
        var bid = new Bid(bidder, money);
        auction.PlaceBid(bid);

        await _auctionChannelWriter.WriteAsync(auction);
        return Result<BidResult>.Success(new BidResult(vehicle!.ToDto(), bid.Bidder, bid.Value.ToDto()));
    }

    private static bool VehicleExists(Vehicle? vehicle) => vehicle is not null;

    private static bool BidIsValid(Money money, Auction auction, Vehicle vehicle) =>
        money.Amount > 0 &&
        (auction.Bids.Count == 0 || money.Amount > auction.CurrentHighestBid.Amount) &&
        money.CurrencyType == vehicle.StartingBid.CurrencyType &&
        money.Amount >= vehicle.StartingBid.Amount;

    private Result<T> LogAndReturnFailure<T>(string message, Problem problem, VehicleId vehicleId, string? bidder = null, Money? money = null)
    {
        if (bidder is not null && money is not null)
            _logger.LogWarning("{Message} for vehicle {VehicleId} by {Bidder}: {Money}", message, vehicleId, bidder, money);
        else
            _logger.LogWarning("{Message}: {VehicleId}", message, vehicleId);

        return Result<T>.Failure(problem);
    }
}