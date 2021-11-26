using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.DeviceRequests
{
    public class GetAllDevicesRequestValidator : AbstractValidator<GetAllDevicesRequest>
    {
        public GetAllDevicesRequestValidator()
        {
            RuleFor(r => r.UserId)
                .Guid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));
        }
    }
}