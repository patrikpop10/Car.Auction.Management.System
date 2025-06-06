using Application.Models.Dtos;

namespace Application.Models.Responses;

public record StartAuctionResponse(MoneyDto StartingBid, DateTime StartTime, Guid AuctionId, VehicleDto Vehicle);