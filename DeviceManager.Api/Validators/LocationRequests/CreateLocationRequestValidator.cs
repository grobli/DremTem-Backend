using System;
using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.LocationRequests
{
    public class CreateLocationRequestValidator : AbstractValidator<CreateLocationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateLocationRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.UserId)
                .Guid();

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

            Transform(@from: r => r, to: r => new { r.Name, UserId = Guid.Parse(r.UserId) })
                .MustAsync(async (key, _) =>
                    await _unitOfWork.Locations.SingleOrDefaultAsync(l =>
                        l.Name == key.Name && l.UserId == key.UserId) is null)
                .WithMessage("Location must have unique Name");
        }
    }
}