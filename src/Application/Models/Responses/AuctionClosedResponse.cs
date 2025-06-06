using Application.Models.Dtos;

namespace Application.Models.Responses;
public record AuctionClosedResponse(Guid VehicleId, string Winner, MoneyDto WinningBid);