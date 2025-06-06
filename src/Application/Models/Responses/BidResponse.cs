using Application.Models.Dtos;

namespace Application.Models.Responses;

public record BidResponse(VehicleDto Vehicle, string Bidder, MoneyDto Amount);