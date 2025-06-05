using Domain.Entities;

namespace Application.DTOs.Responses;

public record BidResponse(VehicleResponse Vehicle, string Bidder, MoneyDto Amount);