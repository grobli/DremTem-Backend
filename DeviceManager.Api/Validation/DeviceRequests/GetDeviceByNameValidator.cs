using FluentValidation;
using Shared.Proto.Device;

namespace DeviceManager.Api.Validation.DeviceRequests
{
    public class GetDeviceByNameValidator : AbstractValidator<GetDeviceByNameRequest>
    {
        public GetDeviceByNameValidator()
        {
            RuleFor(r => r.DeviceName)
                .NotEmpty();

            RuleFor(r => r.Parameters)
                .SetValidator(new GetRequestParametersValidator());
        }
    }
}