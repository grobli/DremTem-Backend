using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.LocationRequests
{
    public class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationRequest>
    {
        public UpdateLocationRequestValidator( )
        {
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

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