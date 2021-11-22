using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Api.Validators
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
                .MustAsync(async (id, _) =>
                {
                    var device = await _unitOfWork.Devices.GetByIdAsync(id);
                    return device is not null;
                }).WithMessage("Device with {PropertyName}={PropertyValue} not found");

            RuleFor(r => r.DisplayName)
                .MaximumLength(DeviceConfiguration.DisplayNameMaxLenght);

            // LocationName validation
            RuleFor(r => r.LocationName)
                .MaximumLength(LocationConfiguration.NameMaxLength);

            Transform(from: r => r, to: value => new { value.Id, value.LocationName })
                .MustAsync(async (x, _) =>
                {
                    var device = await _unitOfWork.Devices.GetByIdAsync(x.Id);
                    var location =
                        await _unitOfWork.Locations.GetByIdAsync(device.UserId, x.LocationName);
                    return location is not null;
                }).WithMessage("{PropertyName} with value: \"{PropertyValue}\" not found.")
                .Unless(r => string.IsNullOrWhiteSpace(r.LocationName));
        }
    }
}