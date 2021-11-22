using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Api.Validators
{
    public class SaveLocationRequestValidator : AbstractValidator<SaveLocationRequest>
    {
        public SaveLocationRequestValidator()
        {
            RuleFor(r => r.UserId)
                .MustBeValidGuid();

            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(LocationConfiguration.NameMaxLength);

            RuleFor(r => r.DisplayName)
                .MaximumLength(LocationConfiguration.DisplayNameMaxLength);

            RuleFor(r => r.Latitude)
                .NotNull()
                .Unless(r => r.Longitude is null)
                .WithMessage("Both Latitude and Longitude must be set simultaneously");

            RuleFor(r => r.Longitude)
                .NotNull()
                .Unless(r => r.Latitude is null)
                .WithMessage("Both Latitude and Longitude must be set simultaneously");
        }
    }
}