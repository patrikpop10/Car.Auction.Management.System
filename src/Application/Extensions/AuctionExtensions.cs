using Application.DTOs;
using Application.DTOs.Responses;
using Domain;
using Domain.Entities;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;
using Domain.Events;

namespace Application.Extensions;

public static class AuctionExtensions
{
    public static StartAuctionResponse ToStartAuctionResponse(this Auction auction, Vehicle vehicle)
    {
        return new StartAuctionResponse(
            vehicle.StartingBid.ToDto(),
            auction.StartedAt,
            auction.VehicleId.Id,
            vehicle.ToDto());
    }
    
    public static AuctionClosed Close(this Auction auction, string winner, Money winningBid)
    {
        return new AuctionClosed(
            auction.VehicleId.Id,
            winner,
            winningBid);
    }
}