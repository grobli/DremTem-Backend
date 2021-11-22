using DeviceManager.Core.Proto;
using FluentValidation;

namespace DeviceManager.Api.Validators
{
    public class GetAllSensorsRequestValidator : AbstractValidator<GetAllSensorsRequest>
    {
        public GetAllSensorsRequestValidator()
        {
            RuleFor(r => r.UserId)
                .MustBeValidGuid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));
        }
    }
}