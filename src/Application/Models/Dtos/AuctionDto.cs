using Domain.Entities.Auctions;

namespace Application.Models.Dtos;

public record AuctionDto(Guid Id, DateTime StartTime, DateTime? EndTime, bool IsActive);