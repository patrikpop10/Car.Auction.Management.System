using Application.DTOs;
using Application.Models.Dtos;

namespace Application.Interfaces;

public record AuctionMonitoringResponse(CarDto Car, BidDto Bid, AuctionDto Auction);