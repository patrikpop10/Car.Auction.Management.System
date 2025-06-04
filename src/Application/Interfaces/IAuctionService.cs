using Application.Common;
using Domain.Entities;
using Domain.Entities.Vehicles;

namespace Application.Services;

public interface IAuctionService
{
    Task<Result<Auction>> StartAuction(VehicleId vehicleId);
    Task<Result<AuctionClosed>> CloseAuction(VehicleId vehicleId);
    Task<Result<BidResult>> PlaceBid(VehicleId vehicleId, string bidder, Money money); 
    
}