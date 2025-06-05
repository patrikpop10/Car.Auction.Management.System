using Application.DTOs;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;

namespace Application.Extensions;

public static class BidExtensions
{
   public static BidDto RequestToDto(this BidRequest bid)
   {
      return new BidDto { Bidder = bid.Bidder, Amount = bid.Amount };
   }
   public static Bid DtoToDomain(this BidDto bid)
      => new Bid(bid.Bidder, bid.Amount.ToDomain());
   
   public static BidDto ToDto(this Bid bid)
   {
      return new BidDto
      {
         Bidder = bid.Bidder,
         Amount = bid.Value.ToDto()
      };
   }
   
   
   
   
   
}