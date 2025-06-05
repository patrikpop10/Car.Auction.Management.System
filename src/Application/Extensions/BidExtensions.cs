using Application.DTOs;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;

namespace Application.Extensions;

public static class BidExtensions
{
   public static BidDto ToDto(this BidRequest bid, VehicleId vehicleId)
   {
      return new BidDto { Id = vehicleId, Bidder = bid.Bidder, Amount = bid.Amount };
   }
   public static BidResponse ToResponse(this BidDto bid, VehicleResponse vehicle)
   {
      return new BidResponse(vehicle, bid.Bidder, bid.Amount);
   }
   public static Bid ToDomain(this BidDto bid)
   {
      return new Bid(bid.Bidder, bid.Amount.ToDomain()); 
   }
   
}