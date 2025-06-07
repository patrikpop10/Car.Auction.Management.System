using Application.Models.Dtos;
using Application.Models.Requests;
using Domain.Entities.Auction;

namespace Application.Extensions;

public static class BidExtensions {
    public static BidDto RequestToDto(this BidRequest bid) => new() { Bidder = bid.Bidder, Amount = bid.Amount };
    public static Bid RequestToDomain(this BidRequest bid)
       => new Bid(bid.Bidder, bid.Amount.ToDomain());

    public static BidDto ToDto(this Bid bid) => new() {
        Bidder = bid.Bidder,
        Amount = bid.Value.ToDto()
    };





}