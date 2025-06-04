using Domain.Entities;

namespace Application.DTOs.Responses;

public record BidResult(VehicleResponse Vehicle, string Bidder, MoneyDto Amount);