using Application.Interfaces;
using Application.Models.Dtos;

namespace Application.Models.Responses;

public record AuctionMonitoringResponse(VehicleDto Vehicle, BidDto Bid, AuctionDto Auction);