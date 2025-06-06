using Application.DTOs;

namespace Application.Interfaces;

public record AuctionMonitoringResponse(CarDto Car, BidDto Bid, AuctionDto Auction);