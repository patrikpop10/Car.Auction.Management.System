using Application.Models.Requests;
using Application.Validators.DtosValidators;
using FluentValidation;

namespace Application.Validators.RequestValidators;

public class VehicleRequestValidator : AbstractValidator<VehicleRequest> {
    public VehicleRequestValidator() {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Vehicle ID is required.");

        RuleFor(x => x.StartingBid)
            .NotNull()
            .WithMessage("Amount is required.")
            .SetValidator(new MoneyDtoValidator());

        RuleFor(x => x.Vehicle)
            .NotNull()
            .WithMessage("Vehicle details are required.")
            .SetValidator(new VehicleDtoValidator());
    }
}