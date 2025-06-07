using Application.Models.Dtos;
using Application.Models.Requests;
using Application.Validators.RequestValidators;

using FluentValidation.TestHelper;

namespace Car.Auction.Management.System.ValidatorTests;

public class VehicleRequestValidatorTests {
    private VehicleRequestValidator _validator;

    [OneTimeSetUp]
    public void SetUp() => _validator = new VehicleRequestValidator();

    [Test]
    public void Validate_WhenIdIsEmpty_ReturnsValidationError() {
        var request = new VehicleRequest {
            Id = Guid.Empty,
            StartingBid = new MoneyDto { Amount = 100, Currency = "USD" },
            Vehicle = new VehicleDto {
                Type = "Car",
                Manufacturer = "Toyota",
                Model = "Corolla",
                Year = 2020,
                NumberOfDoors = 4,
                NumberOfSeats = 5,
                LoadCapacity = 500
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Test]
    public void Validate_WhenStartingBidIsNull_ReturnsValidationError() {
        var request = new VehicleRequest {
            Id = Guid.NewGuid(),
            StartingBid = null!,
            Vehicle = new VehicleDto {
                Type = "Car",
                Manufacturer = "Toyota",
                Model = "Corolla",
                Year = 2020,
                NumberOfDoors = 4,
                NumberOfSeats = 5,
                LoadCapacity = 500
            }
        };
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.StartingBid);
    }

    [Test]
    public void Validate_WhenVehicleRequestIsValid_ReturnsNoValidationErrors() {
        var request = new VehicleRequest {
            Id = Guid.NewGuid(),
            StartingBid = new MoneyDto { Amount = 100, Currency = "USD" },
            Vehicle = new VehicleDto {
                Type = "Car",
                Manufacturer = "Toyota",
                Model = "Corolla",
                Year = 2020,
                NumberOfDoors = 4,
                NumberOfSeats = 5,
                LoadCapacity = 500
            }

        };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}