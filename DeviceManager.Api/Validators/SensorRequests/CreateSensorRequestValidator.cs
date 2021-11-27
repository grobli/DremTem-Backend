using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

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
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(SensorConfiguration.NameMaxLength);

            RuleFor(r => r.DisplayName)
                .MaximumLength(SensorConfiguration.DisplayNameMaxLength);

            RuleFor(r => r.TypeId)
                .NotNull()
                .MustAsync(async (id, _) =>
                    await _unitOfWork.SensorTypes.GetSensorTypeById(id).SingleOrDefaultAsync(_) is not null)
                .WithMessage("referenced sensor type not found.");

            // target device cannot have two sensors with the same name
            Transform(from: r => r, to: r => new { r.Name, r.DeviceId })
                .MustAsync(async (x, _) =>
                    await _unitOfWork.Sensors
                        .SingleOrDefaultAsync(s => s.Name == x.Name && s.DeviceId == x.DeviceId, _) is null)
                .WithMessage("device cannot have two sensors with the same name");
        }
    }
}