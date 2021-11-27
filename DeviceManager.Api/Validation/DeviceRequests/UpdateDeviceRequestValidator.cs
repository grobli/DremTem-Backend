using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validation.DeviceRequests
{
    public class UpdateDeviceRequestValidator : AbstractValidator<UpdateDeviceRequest>
    {
        public UpdateDeviceRequestValidator()
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.LocationId)
                .GreaterThan(0)
                .When(r => r.LocationId is not null);

            RuleFor(r => r.MacAddress)
                .MacAddress()
                .Unless(r => string.IsNullOrWhiteSpace(r.MacAddress));
        }
    }
}