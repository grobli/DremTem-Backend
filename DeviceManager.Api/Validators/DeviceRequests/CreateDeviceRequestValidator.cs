using System;
using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Api.Validators.DeviceRequests
{
    public class CreateDeviceRequestValidator : AbstractValidator<CreateDeviceRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateDeviceRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(DeviceConfiguration.NameMaxLength);

            RuleFor(r => r.UserId)
                .NotEmpty()
                .MustBeValidGuid();

            RuleFor(r => r.DisplayName)
                .MaximumLength(DeviceConfiguration.DisplayNameMaxLenght);

            RuleFor(r => r.MacAddress)
                .NotEmpty()
                .MaximumLength(DeviceConfiguration.MacAddressMaxLength)
                .MacAddress()
                .MustAsync(async (mac, _) =>
                {
                    var macExists =
                        await _unitOfWork.Devices.SingleOrDefaultAsync(d => d.MacAddress == mac) is not null;
                    return !macExists;
                }).WithMessage(
                    "{PropertyName} of device with value: \"{PropertyValue}\" is already used in the system.");

            RuleFor(r => r.Model)
                .MaximumLength(DeviceConfiguration.ModelMaxLength);

            RuleFor(r => r.Manufacturer)
                .MaximumLength(DeviceConfiguration.ManufacturerMaxLength);

            RuleFor(r => r.LocationId)
                .MustAsync(async (id, _) =>
                    // ReSharper disable once PossibleInvalidOperationException
                    await _unitOfWork.Locations.GetByIdAsync((int)id) is not null
                ).WithMessage("{PropertyName} with value: \"{PropertyValue}\" not found.")
                .Unless(r => r.LocationId is null);

            RuleFor(r => r.Sensors)
                .NotEmpty();

            RuleForEach(r => r.Sensors)
                .ChildRules(sensor =>
                {
                    sensor.RuleFor(x => x.Name)
                        .NotEmpty()
                        .MaximumLength(SensorConfiguration.NameMaxLength);

                    sensor.RuleFor(x => x.DisplayName)
                        .MaximumLength(SensorConfiguration.DisplayNameMaxLength);

                    sensor.RuleFor(x => x.TypeId)
                        .MustAsync(async (id, _) => await _unitOfWork.SensorTypes.GetByIdAsync(id) is not null)
                        .WithMessage("{PropertyName} with value: \"{PropertyValue}\" not found");
                });

            Transform(@from: r => r, to: r => new { r.Name, UserId = Guid.Parse(r.UserId) })
                .MustAsync(async (key, _) =>
                    await _unitOfWork.Devices.SingleOrDefaultAsync(
                        d => d.Name == key.Name && d.UserId == key.UserId) is null)
                .WithMessage("Device must have unique Name");
        }
    }
}