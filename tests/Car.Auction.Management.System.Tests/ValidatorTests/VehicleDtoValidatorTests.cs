using Application.Models.Dtos;
using Application.Validators.DtosValidators;

using FluentValidation.TestHelper;

namespace Car.Auction.Management.System.ValidatorTests;

public class VehicleDtoValidatorTests {
    private VehicleDtoValidator _validator;

    [OneTimeSetUp]
    public void SetUp() => _validator = new VehicleDtoValidator();

    [Test]
    public void Validate_WhenTypeIsEmpty_ReturnsValidationError() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = string.Empty,
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2020,
            NumberOfDoors = 4,
            NumberOfSeats = 5,
            LoadCapacity = 500
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Test]
    public void Validate_WhenManufacturerIsEmpty_ReturnsValidationError() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = "Car",
            Manufacturer = string.Empty,
            Model = "Corolla",
            Year = 2020,
            NumberOfDoors = 4,
            NumberOfSeats = 5,
            LoadCapacity = 500
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldHaveValidationErrorFor(x => x.Manufacturer);
    }

    [Test]
    public void Validate_WhenModelIsEmpty_ReturnsValidationError() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = "Car",
            Manufacturer = "Toyota",
            Model = string.Empty,
            Year = 2020,
            NumberOfDoors = 4,
            NumberOfSeats = 5,
            LoadCapacity = 500
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldHaveValidationErrorFor(x => x.Model);
    }

    [Test]
    public void Validate_WhenYearIsInvalid_ReturnsValidationError() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = "Car",
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 1800, // Invalid year
            NumberOfDoors = 4,
            NumberOfSeats = 5,
            LoadCapacity = 500
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldHaveValidationErrorFor(x => x.Year);
    }

    [Test]
    public void Validate_WhenNumberOfDoorsIsNegative_ReturnsValidationError() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = "Car",
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2020,
            NumberOfDoors = -1, // Invalid number of doors
            NumberOfSeats = 5,
            LoadCapacity = 500
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldHaveValidationErrorFor(x => x.NumberOfDoors);
    }

    [Test]
    public void Validate_WhenNumberOfSeatsIsNegative_ReturnsValidationError() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = "Car",
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2020,
            NumberOfDoors = 4,
            NumberOfSeats = -1, // Invalid number of seats
            LoadCapacity = 500
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldHaveValidationErrorFor(x => x.NumberOfSeats);
    }

    [Test]
    public void Validate_WhenLoadCapacityIsNegative_ReturnsValidationError() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = "Truck",
            Manufacturer = "Ford",
            Model = "F-150",
            Year = 2020,
            NumberOfDoors = 2,
            NumberOfSeats = 3,
            LoadCapacity = -100 // Invalid load capacity
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldHaveValidationErrorFor(x => x.LoadCapacity);
    }

    [Test]
    public void Validate_WhenVehicleDtoIsValid_ReturnsNoValidationErrors() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = "Sedan",
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2020,
            NumberOfDoors = 4,
            NumberOfSeats = 5,
            LoadCapacity = 500
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WhenTypeIsInvalid_ReturnsValidationError() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = "InvalidType", // Invalid type
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2020,
            NumberOfDoors = 4,
            NumberOfSeats = 5,
            LoadCapacity = 500
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Test]
    public void Validate_WhenTypeIsValid_ReturnsNoValidationErrors() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = "Hatchback", // Valid type
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2020,
            NumberOfDoors = 4,
            NumberOfSeats = 5,
            LoadCapacity = 500
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void Validate_WhenTypeIsNull_ReturnsValidationError() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = null!, // Null type
            Manufacturer = "Toyota",
            Model = "Corolla",
            Year = 2020,
            NumberOfDoors = 4,
            NumberOfSeats = 5,
            LoadCapacity = 500
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Test]
    public void Validate_WhenManufacturerIsNull_ReturnsValidationError() {
        var vehicle = new VehicleDto {
            Id = Guid.NewGuid(),
            Type = "SUV",
            Manufacturer = null!, // Null manufacturer
            Model = "Corolla",
            Year = 2020,
            NumberOfDoors = 4,
            NumberOfSeats = 5,
            LoadCapacity = 500
        };
        var result = _validator.TestValidate(vehicle);
        result.ShouldHaveValidationErrorFor(x => x.Manufacturer);
    }
}