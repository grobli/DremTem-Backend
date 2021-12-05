using FluentValidation;
using Shared.Proto.SensorType;

namespace DeviceManager.Api.Validation.SensorTypeRequests
{
    public class CreateSensorTypeRequestValidator : AbstractValidator<CreateSensorTypeRequest>
    {
        public CreateSensorTypeRequestValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty();

            RuleFor(r => r.Unit)
                .NotEmpty();

            RuleFor(r => r.IsDiscrete)
                .NotNull();

            RuleFor(r => r.IsSummable)
                .NotNull();
        }
    }
}