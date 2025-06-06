using Application.DTOs;

public record AuctionClosedResponse(Guid VehicleId, string Winner, MoneyDto WinningBid);