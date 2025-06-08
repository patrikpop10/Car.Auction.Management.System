using Application.Models.Dtos;
using Application.Models.Responses;
using Domain.Entities;
using Domain.Entities.Auctions;
using Domain.Entities.Vehicles;
using Domain.Events;

namespace Application.Extensions;

public static class AuctionExtensions {
    public static StartAuctionResponse ToStartAuctionResponse(this Auction auction, Vehicle vehicle)
        => new StartAuctionResponse(
            vehicle.StartingBid.ToDto(),
            auction.StartedAt,
            auction.Id.Id,
            vehicle.ToDto());


    public static AuctionClosed Close(this Auction auction, string winner, Money winningBid) => new AuctionClosed(
        auction.Id.Id,
        auction.VehicleId.Id,
        winner,
        winningBid);


    public static AuctionMonitoringResponse ToMonitoringResponse(this Auction auction, Vehicle vehicle, Bid bid) => new AuctionMonitoringResponse(
        vehicle.ToDto(),
        bid.ToDto(),
        auction.ToDto());

    private static AuctionDto ToDto(this Auction auction) => new AuctionDto(
        auction.Id.Id,
        auction.StartedAt,
        auction.ClosedAt,
        auction.IsActive);
}