using FluentValidation;
using Shared.Proto;

namespace DeviceManager.Api.Validation.SensorRequests
{
    public class GetSensorByNameValidator : AbstractValidator<GetSensorByNameRequest>
    {
        public GetSensorByNameValidator()
        {
            RuleFor(r => r.DeviceId)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(r => r.SensorName)
                .NotEmpty();

            RuleFor(r => r.Parameters)
                .SetValidator(new GetRequestParametersValidator());
        }
    }
}