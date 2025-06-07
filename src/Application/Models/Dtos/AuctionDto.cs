using Domain.Entities.Auction;

namespace Application.Models.Dtos;

public record AuctionDto(AuctionId Id, Guid VehicleId, DateTime StartTime, DateTime? EndTime, bool IsActive);