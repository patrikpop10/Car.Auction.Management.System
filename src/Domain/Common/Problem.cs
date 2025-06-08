using Domain.Entities.Auctions;
using Domain.Entities.Vehicles;

namespace Domain.Common;

public record Problem {
    public int Status { get; private init; }
    public string? Title { get; private init; }
    public string? ErrorMessage { get; private init; }

    public static Problem InvalidVehicleType() => new() {
        Status = 400,
        Title = "InvalidVehicleType",
        ErrorMessage = "The provided vehicle type is invalid."
    };

    public static Problem VehicleNotFound(VehicleId vehicleId) => new() {
        Status = 404,
        Title = "NotFound",
        ErrorMessage = $"The requested vehicle with Id {vehicleId} was not found."
    };

    public static Problem DuplicateVehicle(VehicleId vehicleId) => new() {
        Status = 409,
        Title = "DuplicateVehicle",
        ErrorMessage = $"A vehicle with the ID {vehicleId.Id} already exists."
    };

    public static Problem AuctionForVehicleAlreadyActive(VehicleId auctionId) => new() {
        Status = 409,
        Title = "AuctionAlreadyActive",
        ErrorMessage = $"An auction is already active for the vehicle with ID {auctionId.Id}."
    };

    public static Problem AuctionForVehicleNotActive(VehicleId vehicleId) => new() {
        Status = 404,
        Title = "AuctionNotActive",
        ErrorMessage = $"No active auction found for the vehicle with ID {vehicleId.Id}."
    };

    public static Problem InvalidBidAmount() => new() {
        Status = 400,
        Title = "InvalidBidAmount",
        ErrorMessage = "The bid amount must be greater than the current highest bid."
    };

    public static Problem InternalServerError() => new() {
        Status = 500,
        Title = "InternalServerError",
        ErrorMessage = "An unexpected error occurred on the server."
    };

    public static Problem AuctionNotFound(AuctionId auctionId) => new() {
        Status = 404,
        Title = "AuctionNotFound",
        ErrorMessage = $"No auction found for the vehicle with ID {auctionId.Id}."
    };

    public static Problem VehicleIdMismatch(VehicleId vehicleId, VehicleId vehicleId1) => new() {
        Status = 400,
        Title = "VehicleIdMismatch",
        ErrorMessage = $"The provided vehicle ID {vehicleId.Id} does not match the auction's vehicle ID {vehicleId.Id}."
    };
    public static Problem Closed(AuctionId auctionId) => new() {
        Status = 400,
        Title = "Closed",
        ErrorMessage = $"The auction {auctionId.Id} is closed."
    };
}