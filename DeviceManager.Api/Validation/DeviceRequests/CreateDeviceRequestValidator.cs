using FluentValidation;
using Shared.Extensions;
using Shared.Proto;

namespace DeviceManager.Api.Validation.DeviceRequests
{
    public class CreateDeviceRequestValidator : AbstractValidator<CreateDeviceRequest>
    {
        public CreateDeviceRequestValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty();

            RuleFor(r => r.LocationId)
                .GreaterThan(0)
                .When(r => r.LocationId is not null);

            RuleFor(r => r.MacAddress)
                .MacAddress()
                .Unless(r => string.IsNullOrWhiteSpace(r.MacAddress));

            RuleFor(r => r.Sensors)
                .NotEmpty();

            RuleForEach(r => r.Sensors)
                .ChildRules(sensor =>
                {
                    sensor.RuleFor(x => x.Name)
                        .NotEmpty();
                });

            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();
        }
    }
}