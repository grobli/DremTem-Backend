using FluentValidation;
using Shared.Extensions;
using Shared.Proto.Sensor;

namespace DeviceManager.Api.Validation.SensorRequests
{
    public class UpdateSensorRequestValidator : AbstractValidator<UpdateSensorRequest>
    {
        public UpdateSensorRequestValidator()
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.DisplayName)
                .NotEmpty();

            RuleFor(r => r.TypeId)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(r => r.Id)
                .NotEmpty()
                .GreaterThan(0);
        }
    }
}