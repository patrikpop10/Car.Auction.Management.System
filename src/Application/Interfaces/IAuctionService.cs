using Application.DTOs;
using Application.DTOs.Responses;
using Domain;
using Domain.Common;
using Domain.Entities;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;
using Domain.Events;

namespace Application.Interfaces;

public interface IAuctionService
{
    Task<Result<StartAuctionResponse>> StartAuction(VehicleId vehicleId);
    Task<Result<AuctionClosedResponse>> CloseAuction(VehicleId vehicleId);
    Task<Result<BidResponse>> PlaceBid(BidDto bidDto, VehicleId vehicleId); 
    
}