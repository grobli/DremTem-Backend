using System;
using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Api.Validators
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

            //TODO: Check if user exists in UserIdentityService
            RuleFor(r => r.UserId)
                .NotEmpty()
                .MustBeValidGuid();

            RuleFor(r => r.DisplayName)
                .MaximumLength(DeviceConfiguration.DisplayNameMaxLenght);


            // LocationName validation
            RuleFor(r => r.LocationName)
                .MaximumLength(LocationConfiguration.NameMaxLength);
            Transform(from: r => r, to: value => new { UserId = Guid.Parse(value.UserId), value.LocationName })
                .MustAsync(async (locationKey, _) =>
                {
                    var location =
                        await _unitOfWork.Locations.GetByIdAsync(locationKey.UserId, locationKey.LocationName);
                    return location is not null;
                }).WithMessage("{PropertyName} with value: \"{PropertyValue}\" not found.")
                .Unless(r => string.IsNullOrWhiteSpace(r.LocationName));


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

                    sensor.RuleFor(x => x.TypeName)
                        .NotEmpty()
                        .MaximumLength(SensorTypeConfiguration.NameMaxLength)
                        .MustAsync(async (type, _) =>
                        {
                            var sensorType = await _unitOfWork.SensorTypes.GetByIdAsync(type);
                            return sensorType is not null;
                        }).WithMessage("{PropertyName} with value: \"{PropertyValue}\" not found");
                });
        }
    }
}