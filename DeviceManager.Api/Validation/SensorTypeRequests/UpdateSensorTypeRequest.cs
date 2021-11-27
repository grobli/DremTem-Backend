using DeviceManager.Core.Proto;
using FluentValidation;

namespace DeviceManager.Api.Validation.SensorTypeRequests
{
    public class UpdateSensorTypeRequestValidator : AbstractValidator<UpdateSensorTypeRequest>
    {
        public UpdateSensorTypeRequestValidator()
        {
            RuleFor(r => r.Unit)
                .NotEmpty();

            RuleFor(r => r.IsDiscrete)
                .NotNull();

            RuleFor(r => r.IsSummable)
                .NotNull();
        }
    }
}