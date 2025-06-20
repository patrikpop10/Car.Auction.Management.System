using Application.Models.Dtos;
using Domain.Entities.Vehicles;
using FluentValidation;

namespace Application.Validators.DtosValidators;

public class VehicleDtoValidator : AbstractValidator<VehicleDto> {
    private static readonly HashSet<string> ValidVehicleTypes = [
        nameof(Hatchback),
        nameof(Sedan),
        nameof(SUV),
        nameof(Truck)
    ];

    public VehicleDtoValidator() {

        RuleFor(vehicle => vehicle.Id)
            .NotEmpty()
            .WithMessage("Vehicle ID is required.");

        RuleFor(vehicle => vehicle.Type)
            .NotEmpty()
            .WithMessage("Vehicle type is required.");

        RuleFor(vehicle => vehicle.Manufacturer)
            .NotEmpty()
            .WithMessage("Manufacturer is required.");

        RuleFor(vehicle => vehicle.Model)
            .NotEmpty()
            .WithMessage("Model is required.");

        RuleFor(vehicle => vehicle.Year)
            .InclusiveBetween(1886, DateTime.Now.Year).WithMessage("Year must be between 1886 and the current year.");

        RuleFor(vehicle => vehicle.NumberOfDoors)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Number of doors must be greater than 0 and less than 100.")
            .When(vehicle => vehicle.NumberOfDoors.HasValue);

        RuleFor(vehicle => vehicle.NumberOfSeats)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(100)
            .WithMessage("Number of seats must be zero or greater.")
            .When(vehicle => vehicle.NumberOfSeats.HasValue);

        RuleFor(vehicle => vehicle.LoadCapacity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Load capacity must be zero or greater.")
            .When(vehicle => vehicle.LoadCapacity.HasValue);

        RuleFor(vehicle => vehicle.Type)
            .Must(BeAValidVehicleType)
            .WithMessage("Vehicle type must be one of the following: Hatchback, Sedan, SUV, Truck.");

    }
    private static bool BeAValidVehicleType(string arg) => ValidVehicleTypes.Contains(arg);
}