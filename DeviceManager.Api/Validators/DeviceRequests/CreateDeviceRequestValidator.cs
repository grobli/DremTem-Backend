using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

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
                        .MustAsync(async (id, _) =>
                            await _unitOfWork.SensorTypes.GetSensorTypeById(id)
                                .SingleOrDefaultAsync(_) is not null)
                        .WithMessage("{PropertyName} with value: \"{PropertyValue}\" not found");
                });
        }
    }
}