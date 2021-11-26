using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Api.Validators.SensorRequests
{
    public class CreateSensorRequestValidator : AbstractValidator<CreateSensorRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSensorRequestValidator(IUnitOfWork unitOfWork)
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

            // check if referenced device exists and if it does then check if it belongs to the user
            Transform(from: r => r, to: r => new { r.DeviceId, r.UserId })
                .MustAsync(async (x, _) =>
                {
                    var device = await _unitOfWork.Devices.GetByIdAsync(x.DeviceId);
                    if (device is null) return false;
                    if (string.IsNullOrWhiteSpace(x.UserId)) return true;
                    return device.UserId.ToString() == x.UserId;
                }).WithMessage("referenced device not found");

            RuleFor(r => r.TypeId)
                .NotNull()
                .MustAsync(async (id, _) => await _unitOfWork.Sensors.GetByIdAsync(id) is not null)
                .WithMessage("referenced sensor type not found.");

            // target device cannot have two sensors with the same name
            Transform(from: r => r, to: r => new { r.Name, r.DeviceId })
                .MustAsync(async (x, _) =>
                    await _unitOfWork.Sensors
                        .SingleOrDefaultAsync(s => s.Name == x.Name && s.DeviceId == x.DeviceId) is null)
                .WithMessage("device cannot have two sensors with the same name");
        }
    }
}