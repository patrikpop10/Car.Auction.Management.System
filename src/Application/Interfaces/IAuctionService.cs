using Application.Models.Requests;
using Application.Models.Responses;
using Domain.Common;
using Domain.Entities.Vehicles;

namespace Application.Interfaces;

public interface IAuctionService {
    Task<Result<StartAuctionResponse>> StartAuction(VehicleId vehicleId);
    Task<Result<AuctionClosedResponse>> CloseAuction(VehicleId vehicleId);
    Task<Result<BidResponse>> PlaceBid(BidRequest bidRequest, VehicleId vehicleId);

}