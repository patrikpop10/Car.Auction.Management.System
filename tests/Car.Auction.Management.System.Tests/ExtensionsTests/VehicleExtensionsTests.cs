using Application.Extensions;
using Application.Models.Dtos;
using Application.Models.Requests;
using Domain.Entities;
using Domain.Entities.Vehicles;
using FluentAssertions;

namespace Car.Auction.Management.System.ExtensionsTests;

[TestFixture]
public class VehicleExtensionsTests {
    [Test]
    public void ToDto_WhenVehicleIsProvided_ReturnsCorrectVehicleDto() {
        // Arrange
        var id = Guid.NewGuid();
        var vehicle = new Sedan(new VehicleId(id), "Toyota", "Corolla", 2020, new Money(100, CurrencyType.USD), 5);

        // Act
        var dto = vehicle.ToDto();

        // Assert
        dto.Should().NotBeNull();
        dto.Manufacturer.Should().Be(vehicle.Manufacturer);
        dto.Model.Should().Be(vehicle.Model);
        dto.Year.Should().Be(vehicle.Year);
    }

    [Test]
    public void RequestToDomain_WhenVehicleRequestIsProvided_ReturnsCorrectVehicle() {
        // Arrange
        var id = Guid.NewGuid();
        var vehicleRequest = new VehicleRequest {
            Vehicle = new VehicleDto { Id = Guid.NewGuid(), Type = nameof(Sedan), Manufacturer = "Toyota", Model = "Corolla", Year = 2020 },
            StartingBid = new MoneyDto { Amount = 100, Currency = "USD" }
        };

        // Act
        var domain = vehicleRequest.RequestToDomain();

        // Assert
        domain.Should().NotBeNull();
        domain.Should().BeOfType<Sedan>();
        domain.Manufacturer.Should().Be(vehicleRequest.Vehicle.Manufacturer);
        domain.Model.Should().Be(vehicleRequest.Vehicle.Model);
        domain.Year.Should().Be(vehicleRequest.Vehicle.Year);
        domain.StartingBid.Amount.Should().Be(vehicleRequest.StartingBid.Amount);
        domain.StartingBid.CurrencyType.ToString().Should().Be(vehicleRequest.StartingBid.Currency);
    }

    [Test]
    public void DomainToResponse_WhenVehicleIsProvided_ReturnsCorrectSearchVehicleResponse() {
        // Arrange
        var sedan = new Sedan(new VehicleId(Guid.NewGuid()), "Toyota", "Corolla", 2020, new Money(100, CurrencyType.USD), 4);

        // Act
        var response = sedan.DomainToResponse();

        // Assert
        response.Should().NotBeNull();
        response.Vehicle.Type.Should().Be(nameof(Sedan));
        response.Vehicle.Manufacturer.Should().Be(sedan.Manufacturer);
        response.Vehicle.Model.Should().Be(sedan.Model);
        response.Vehicle.Year.Should().Be(sedan.Year);
        response.Vehicle.NumberOfDoors.Should().Be(sedan.NumberOfDoors);
        response.StartingBid.Amount.Should().Be(sedan.StartingBid.Amount);
        response.StartingBid.Currency.Should().Be(sedan.StartingBid.CurrencyType.ToString());
    }
}