using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Data.Validation
{
    public class SensorValidator : AbstractValidator<Sensor>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SensorValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            SetupNameRules();
            SetupDisplayNameRules();
            SetupDeviceIdRules();
            SetupTypeIdRules();
            SetupLastModifiedRules();
            SetupCreatedRules();
        }

        private void SetupNameRules()
        {
            RuleFor(s => s.Name)
                .NotEmpty()
                .MaximumLength(SensorConfiguration.NameMaxLength);

            // parent device cannot have two sensors with the same name
            Transform(from: s => s, to: s => s)
                .MustAsync(async (s, ct) =>
                    await _unitOfWork.Sensors
                        .SingleOrDefaultAsync(
                            x => x.Id != s.Id && x.Name == s.Name && x.DeviceId == s.DeviceId, ct) is null)
                .WithMessage("device cannot have two sensors with the same name");
        }

        private void SetupDisplayNameRules()
        {
            RuleFor(s => s.DisplayName)
                .MaximumLength(SensorConfiguration.DisplayNameMaxLength);
        }

        private void SetupDeviceIdRules()
        {
            RuleFor(s => s.DeviceId)
                .NotEmpty()
                .MustAsync(async (devId, ct) =>
                    // device must exist
                    await _unitOfWork.Devices.SingleOrDefaultAsync(d => d.Id == devId, ct) is not null
                ).WithMessage("device referred by {PropertyName} = {PropertyValue} does not exist");
        }

        private void SetupTypeIdRules()
        {
            RuleFor(s => s.TypeId)
                .NotEmpty()
                .MustAsync(async (typeId, ct) =>
                    // sensor type must exist
                    await _unitOfWork.SensorTypes.SingleOrDefaultAsync(st => st.Id == typeId, ct) is not null
                ).WithMessage("sensor type referred by {PropertyName} = {PropertyValue} does not exist");
        }

        private void SetupLastModifiedRules()
        {
            RuleFor(s => s.LastModified)
                .GreaterThanOrEqualTo(s => s.Created)
                .Unless(s => s.LastModified is null);
        }

        private void SetupCreatedRules()
        {
            RuleFor(s => s.Created)
                .NotEmpty();
        }
    }
}