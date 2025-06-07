using Application.Extensions;
using Domain.Entities;
using Domain.Entities.Vehicles;
using FluentAssertions;

namespace Car.Auction.Management.System.ExtensionsTests;

public class AuctionExtensionsTests {

    [Test]
    public void ToStartAuctionResponse_WhenAuctionAndVehicleAreProvided_ReturnsCorrectResponse() {
        // Arrange
        var id = new VehicleId(Guid.NewGuid());
        var auction = new Domain.Entities.Auction.Auction(id);
        var vehicle = new Sedan(id, "Toyota", "Camry", 2020, new Money(15000, CurrencyType.USD), 5);

        // Act
        var response = auction.ToStartAuctionResponse(vehicle);

        // Assert
        response.Should().NotBeNull();
        response.Vehicle.Should().NotBeNull();
        response.Vehicle.Manufacturer.Should().Be(vehicle.Manufacturer);
        response.Vehicle.Model.Should().Be(vehicle.Model);
        response.Vehicle.Year.Should().Be(vehicle.Year);
        response.StartingBid.Amount.Should().Be(vehicle.StartingBid.Amount);
        response.StartingBid.Currency.Should().Be(vehicle.StartingBid.CurrencyType.ToString());
        response.AuctionId.Should().Be(id.Id);
        response.StartTime.Should().Be(auction.StartedAt);
        response.AuctionId.Should().NotBeEmpty();

    }

    [Test]
    public void Close_WhenAuctionAndWinningBidAreProvided_ReturnsCorrectAuctionClosed() {
        // Arrange
        var id = new VehicleId(Guid.NewGuid());
        var auction = new Domain.Entities.Auction.Auction(id);
        var winningBid = new Money(200, CurrencyType.USD);

        // Act
        var closed = auction.Close("John", winningBid);

        // Assert

        closed.Should().NotBeNull();
        closed.Vehicle.Should().Be(id.Id);
        closed.Winner.Should().Be("John");
        closed.WinningBid.Amount.Should().Be(winningBid.Amount);
        closed.WinningBid.CurrencyType.Should().Be(winningBid.CurrencyType);
        closed.AuctionId.Should().Be(auction.Id.Id);
    }
}