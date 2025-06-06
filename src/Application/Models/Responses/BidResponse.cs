using Application.Models.Dtos;

namespace Application.Models.Responses;

public record BidResponse(VehicleResponse Vehicle, string Bidder, MoneyDto Amount);