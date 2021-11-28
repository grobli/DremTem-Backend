using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Data.Validation
{
    public class LocationValidator : AbstractValidator<Location>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocationValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            SetupNameRules();
            SetupDisplayNameRules();
            SetupLatLonRules();
            SetupLastModifiedRules();
            SetupCreatedRules();
            SetupUserIdRules();
        }

        private void SetupNameRules()
        {
            RuleFor(l => l.Name)
                .NotEmpty()
                .MaximumLength(LocationConfiguration.NameMaxLength);

            Transform(l => l, l => l)
                .MustAsync(async (l, ct) =>
                    await _unitOfWork.Locations.SingleOrDefaultAsync(x =>
                        x.Id != l.Id && x.Name == l.Name && x.UserId == l.UserId, ct) is null)
                .WithMessage("location must have unique name");
        }

        private void SetupDisplayNameRules()
        {
            RuleFor(l => l.DisplayName)
                .MaximumLength(LocationConfiguration.DisplayNameMaxLength);
        }

        private void SetupLatLonRules()
        {
            RuleFor(l => l.Latitude)
                .NotNull()
                .Unless(l => l.Longitude is null)
                .WithMessage("latitude and longitude must both have a value or be null");

            RuleFor(l => l.Longitude)
                .NotNull()
                .Unless(l => l.Latitude is null)
                .WithMessage("latitude and longitude must both have a value or be null");
        }

        private void SetupLastModifiedRules()
        {
            RuleFor(l => l.LastModified)
                .GreaterThanOrEqualTo(l => l.Created)
                .Unless(l => l.LastModified is null);
        }

        private void SetupCreatedRules()
        {
            RuleFor(l => l.Created)
                .NotEmpty();
        }

        private void SetupUserIdRules()
        {
            RuleFor(l => l.UserId)
                .NotEmpty();
        }
    }
}