using Application.Interfaces;
using Application.Models.Responses;
using Domain.Entities;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;
using Domain.Events;

namespace Application.Extensions;

public static class AuctionExtensions {
    public static StartAuctionResponse ToStartAuctionResponse(this Auction auction, Vehicle vehicle)
        => new StartAuctionResponse(
            vehicle.StartingBid.ToDto(),
            auction.StartedAt,
            auction.VehicleId.Id,
            vehicle.ToDto());


    public static AuctionClosed Close(this Auction auction, string winner, Money winningBid) => new AuctionClosed(
        auction.VehicleId.Id,
        winner,
        winningBid);


    public static AuctionMonitoringResponse ToMonitoringResponse(this Auction auction, Vehicle vehicle, Bid bid) => new AuctionMonitoringResponse(
        vehicle.ToDto(),
        bid.ToDto(),
        auction.ToDto());

    private static AuctionDto ToDto(this Auction auction) => new AuctionDto(
        auction.VehicleId.Id,
        auction.StartedAt,
        auction.ClosedAt,
        auction.IsActive);
}