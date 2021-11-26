using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.SensorRequests
{
    public class GetAllSensorsRequestValidator : AbstractValidator<GetAllSensorsRequest>
    {
        public GetAllSensorsRequestValidator()
        {
            RuleFor(r => r.UserId)
                .Guid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));
        }
    }
}