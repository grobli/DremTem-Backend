using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Api.Validators
{
    public class GetLocationRequestValidator : AbstractValidator<GetLocationRequest>
    {
        public GetLocationRequestValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(LocationConfiguration.NameMaxLength);

            RuleFor(r => r.UserId)
                .MustBeValidGuid();
        }
    }
}