using Domain.Entities.Vehicles;

namespace Domain.Common;

public record Problem {
    public int Status { get; init; }
    public string? Title { get; init; }
    public string? ErrorMessage { get; init; }

    public static Problem InvalidVehicleType() {
        return new Problem {
            Status = 400,
            Title = "InvalidVehicleType",
            ErrorMessage = "The provided vehicle type is invalid."
        };
    }



    public static Problem VehicleNotFound(VehicleId vehicleId) {
        return new Problem() {
            Status = 404,
            Title = "NotFound",
            ErrorMessage = $"The requested vehicle with Id {vehicleId} was not found."
        };
    }

    public static Problem DuplicateVehicle(VehicleId vehicleId) {
        return new Problem() {
            Status = 409,
            Title = "DuplicateVehicle",
            ErrorMessage = $"A vehicle with the ID {vehicleId.Id} already exists."
        };
    }

    public static Problem AuctionAlreadyActive(VehicleId vehicleId) {
        return new Problem() {
            Status = 409,
            Title = "AuctionAlreadyActive",
            ErrorMessage = $"An auction is already active for the vehicle with ID {vehicleId.Id}."
        };
    }

    public static Problem AuctionNotActive(VehicleId vehicleId) {
        return new Problem {
            Status = 404,
            Title = "AuctionNotActive",
            ErrorMessage = $"No active auction found for the vehicle with ID {vehicleId.Id}."
        };
    }

    public static Problem InvalidBidAmount() {
        return new Problem {
            Status = 400,
            Title = "InvalidBidAmount",
            ErrorMessage = "The bid amount must be greater than the current highest bid."
        };
    }

    public static Problem InternalServerError() {
        return new Problem {
            Status = 500,
            Title = "InternalServerError",
            ErrorMessage = "An unexpected error occurred on the server."
        };
    }

    public static Problem AuctionNotFound(VehicleId vehicleId) {
        return new Problem {
            Status = 404,
            Title = "AuctionNotFound",
            ErrorMessage = $"No auction found for the vehicle with ID {vehicleId.Id}."
        };
    }

    public static Problem VehicleIdMismatch(VehicleId vehicleId, VehicleId vehicleId1) {
        return new Problem {
            Status = 400,
            Title = "VehicleIdMismatch",
            ErrorMessage = $"The provided vehicle ID {vehicleId1.Id} does not match the auction's vehicle ID {vehicleId.Id}."
        };
    }
}