using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Api.Validators
{
    public class SaveSensorRequestValidator : AbstractValidator<SaveSensorRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SaveSensorRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(SensorConfiguration.NameMaxLength);

            RuleFor(r => r.DisplayName)
                .MaximumLength(SensorConfiguration.DisplayNameMaxLength);

            RuleFor(r => r.DeviceId)
                .NotNull()
                .MustAsync(async (id, _) =>
                {
                    var device = await _unitOfWork.Devices.GetByIdAsync(id);
                    return device is not null;
                }).WithMessage("{PropertyName} with value: \"{PropertyValue}\" not found.");

            RuleFor(r => r.TypeName)
                .NotEmpty()
                .MustAsync(async (type, _) =>
                {
                    var sensorType = await _unitOfWork.SensorTypes.GetByIdAsync(type);
                    return sensorType is not null;
                }).WithMessage("{PropertyName} with value: \"{PropertyValue}\" not found.");
        }
    }
}