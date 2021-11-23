using DeviceManager.Core.Proto;
using FluentValidation;

namespace DeviceManager.Api.Validators
{
    public class GetDeviceRequestValidator : AbstractValidator<GetDeviceRequest>
    {
        public GetDeviceRequestValidator()
        {
            RuleFor(r => r.UserId)
                .MustBeValidGuid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));
        }
    }
}