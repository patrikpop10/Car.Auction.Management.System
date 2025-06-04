using System.Threading.Channels;
using Application.Common;
using Application.DTOs;
using Application.Extensions;
using Domain.Entities;
using Domain.Entities.Vehicles;
using Domain.Repositories;

namespace Application.Services;

public class AuctionService : IAuctionService
{
    private readonly IVehicleRepository _vehicleRepo;
    private readonly IAuctionRepository _auctionRepo;
    private ChannelWriter<Auction> _auctionChannelWriter;

    public AuctionService(IVehicleRepository vehicleRepo, IAuctionRepository auctionRepo, ChannelWriter<Auction> auctionChannelWriter)
    {
        _vehicleRepo = vehicleRepo;
        _auctionRepo = auctionRepo;
        _auctionChannelWriter = auctionChannelWriter;
    }

    public async Task<Result<Auction>> StartAuction(VehicleId vehicleId)
    {
        if (await _vehicleRepo.GetById(vehicleId) is null)
        {
            return Result<Auction>.Failure(Problem.VehicleNotFound(vehicleId));
        }

        if (await _auctionRepo.IsAuctionActive(vehicleId))
        {
            return Result<Auction>.Failure(Problem.AuctionAlreadyActive(vehicleId));
        }

        var auction = new Auction(vehicleId);
        await _auctionRepo.Add(auction);
        return Result<Auction>.Success(auction);

    }


    public async Task<Result<AuctionClosed>> CloseAuction(VehicleId vehicleId)
    {
        var auction = await _auctionRepo.GetActiveByVehicleId(vehicleId);
        if (auction is null)
        {
            return Result<AuctionClosed>.Failure(Problem.AuctionNotActive(vehicleId));
        }

        var closed = await _auctionRepo.CloseAuction(auction);
        return !closed ? Result<AuctionClosed>.Failure(Problem.AuctionNotFound(vehicleId)) : auction.Close();
    }

    public async Task<Result<BidResult>> PlaceBid(VehicleId vehicleId, string bidder, Money money)
    {
        var auction = await _auctionRepo.GetActiveByVehicleId(vehicleId);
        var vehicle = await _vehicleRepo.GetById(vehicleId);
        
        if (vehicle is null)
        {
            return Result<BidResult>.Failure(Problem.VehicleNotFound(vehicleId));
        }
        
        if (auction is null)
        {
            return Result<BidResult>.Failure(Problem.AuctionNotActive(vehicleId));
        }

        if (!BidIsValid(money, auction, vehicle))
        {
            return Result<BidResult>.Failure(Problem.InvalidBidAmount());
        }

        var bid = new Bid(bidder, money);
        auction.PlaceBid(bid);
        //await _auctionRepo.Update(auction);


        await _auctionChannelWriter.WriteAsync(auction);
        return Result<BidResult>.Success(new BidResult(vehicle, bid.Bidder, bid.Value));

    }


    private static bool BidIsValid(Money money, Auction auction, Vehicle vehicle)
    //check if the bid is valid according to the rules
    //the bid must be greater than the current highest bid, greater than 0, and greater than the starting bid
    // the bid must be in the same currency as the current highest bid if there is one
    =>
        money.Amount > 0 &&
        (auction.Bids.Count == 0 || money.Amount > auction.CurrentHighestBid.Amount) &&
        money.CurrencyType == vehicle.StartingBid.CurrencyType &&
        money.Amount >= vehicle.StartingBid.Amount;


}

public record BidResult(Vehicle Vehicle, string Bidder, Money Amount);