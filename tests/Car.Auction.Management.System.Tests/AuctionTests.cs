using Domain.Common;
using Domain.Entities;
using Domain.Entities.Auction;
using Domain.Entities.Vehicles;
using FluentAssertions;

namespace Car.Auction.Management.System;

[TestFixture]
public class AuctionTests {

    [Test]
    public void Auction_WhenConstructed_ShouldHaveCorrectValues() {
        // Arrange
        var vehicleId = new VehicleId(Guid.NewGuid());

        // Act
        var auction = new Domain.Entities.Auction.Auction(vehicleId);

        // Assert
        auction.Should().NotBeNull();
        auction.VehicleId.Should().Be(vehicleId);
        auction.IsActive.Should().BeTrue();
    }

    [Test]
    public void AuctionPlaceBid_WhenVehicleIdMismatch_ShouldReturnFailure() {
        // Arrange
        var auction = new Domain.Entities.Auction.Auction(new VehicleId(Guid.NewGuid()));
        var vehicle = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Camry", 2020, new Money(15000, CurrencyType.USD), 5);
        var bid = new Bid("bidder1", new Money(20000, CurrencyType.USD));

        // Act
        var result = auction.PlaceBid(bid, vehicle);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Problem.Should().NotBeNull();
        result.Problem!.Status.Should().Be(Problem.VehicleIdMismatch(auction.VehicleId, vehicle.Id).Status);
        result.Problem.ErrorMessage.Should().Be(Problem.VehicleIdMismatch(auction.VehicleId, vehicle.Id).ErrorMessage);
        result.Problem.Title.Should().Be(Problem.VehicleIdMismatch(auction.VehicleId, vehicle.Id).Title);
    }

    [Test]
    public void AuctionPlaceBid_WhenBidIsValid_ShouldAddBidAndReturnSuccess() {
        // Arrange
        var auction = new Domain.Entities.Auction.Auction(new VehicleId(Guid.NewGuid()));
        var vehicle = new Sedan(auction.VehicleId, "Toyota", "Camry", 2020, new Money(15000, CurrencyType.USD), 5);
        var bid = new Bid("bidder1", new Money(20000, CurrencyType.USD));

        // Act
        var result = auction.PlaceBid(bid, vehicle);

        // Assert
        result.IsSuccess.Should().BeTrue();
        auction.Bids.Should().Contain(bid);
    }



    [Test]
    public void AuctionCloseAuction_WhenAuctionIsAlreadyClosed_ShouldReturnError() {
        // Arrange

        var auction = new Domain.Entities.Auction.Auction(new VehicleId(Guid.NewGuid()));
        auction.Close(); // Close the auction to set it to inactive

        // Act

        var result = auction.Close();
        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Problem.Should().NotBeNull();
        result.Problem!.Status.Should().Be(Problem.AuctionForVehicleNotActive(auction.VehicleId).Status);
        result.Problem.ErrorMessage.Should().Be(Problem.AuctionForVehicleNotActive(auction.VehicleId).ErrorMessage);
        result.Problem.Title.Should().Be(Problem.AuctionForVehicleNotActive(auction.VehicleId).Title);

    }
}