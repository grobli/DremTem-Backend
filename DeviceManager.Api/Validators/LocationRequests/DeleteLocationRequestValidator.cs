using DeviceManager.Core;
using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.LocationRequests
{
    public class DeleteLocationRequestValidator : AbstractValidator<DeleteLocationRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLocationRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.UserId)
                .Guid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));

            RuleFor(r => r.Id)
                .MustAsync(async (id, _) => await _unitOfWork.Locations.GetByIdAsync(id) is not null)
                .WithMessage("Location with {PropertyName} = \"{PropertyValue}\" not found");

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