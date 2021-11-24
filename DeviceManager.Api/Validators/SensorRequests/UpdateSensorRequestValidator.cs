using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Api.Validators.SensorRequests
{
    public class UpdateSensorRequestValidator : AbstractValidator<UpdateSensorRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSensorRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.Id)
                .MustAsync(async (id, _) => await _unitOfWork.Sensors.GetByIdAsync(id) is not null)
                .WithMessage("Sensor with {PropertyName} = \"{PropertyValue}\" not found");

            RuleFor(r => r.DisplayName)
                .MaximumLength(SensorConfiguration.DisplayNameMaxLength);

            RuleFor(r => r.TypeId)
                .NotNull()
                .MustAsync(async (id, _) => await _unitOfWork.Sensors.GetByIdAsync(id) is not null)
                .WithMessage("SensorType with {PropertyName} = \"{PropertyValue}\" not found.");
        }
    }
}