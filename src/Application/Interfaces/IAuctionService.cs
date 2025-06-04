using Application.DTOs.Responses;
using Domain;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;

namespace Application.Interfaces;

public interface IAuctionService
{
    Task<Result<StartAuctionResponse>> StartAuction(VehicleId vehicleId);
    Task<Result<AuctionClosed>> CloseAuction(VehicleId vehicleId);
    Task<Result<BidResult>> PlaceBid(VehicleId vehicleId, string bidder, Money money); 
    
}