using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.LocationRequests
{
    public class UpdateLocationRequestValidator : AbstractValidator<UpdateLocationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLocationRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.Id)
                .MustAsync(async (id, _) => await _unitOfWork.Locations.GetByIdAsync(id) is not null)
                .WithMessage("Location with {PropertyName} = \"{PropertyValue}\" not found");

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

            // if userId specified then location.userId must match
            Transform(@from: r => r, to: r => new { r.Id, r.UserId })
                .MustAsync(async (x, _) =>
                    string.IsNullOrWhiteSpace(x.UserId) ||
                    await _unitOfWork.Locations
                        .SingleOrDefaultAsync(l => l.Id == x.Id && l.UserId.ToString() == x.UserId) is not null
                ).WithMessage("Location not found");
        }
    }
}