using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Api.Validators.DeviceRequests
{
    public class UpdateDeviceRequestValidator : AbstractValidator<UpdateDeviceRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDeviceRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.Id)
                .MustAsync(async (id, _) => await _unitOfWork.Devices.GetByIdAsync(id) is not null)
                .WithMessage("Device with {PropertyName}={PropertyValue} not found");

            RuleFor(r => r.DisplayName)
                .MaximumLength(DeviceConfiguration.DisplayNameMaxLenght);

            RuleFor(r => r.LocationId)
                .MustAsync(async (id, _) =>
                    // ReSharper disable once PossibleInvalidOperationException
                    await _unitOfWork.Locations.GetByIdAsync((int)id) is not null
                ).WithMessage("{PropertyName} with value: \"{PropertyValue}\" not found.")
                .Unless(r => r.LocationId is null);

            RuleFor(r => r.UserId)
                .MustBeValidGuid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));

            // if userId specified then device.userId must match
            Transform(from: r => r, to: r => new { r.Id, r.UserId })
                .MustAsync(async (x, _) =>
                    string.IsNullOrWhiteSpace(x.UserId) ||
                    await _unitOfWork.Devices
                        .SingleOrDefaultAsync(d => d.Id == x.Id && d.UserId.ToString() == x.UserId) is not null
                ).WithMessage("Device not found");
        }
    }
}