using DeviceManager.Core.Proto;
using FluentValidation;

namespace DeviceManager.Api.Validators
{
    public class GenericGetRequestValidator : AbstractValidator<GenericGetRequest>
    {
        public GenericGetRequestValidator()
        {
            RuleFor(r => r.Parameters)
                .SetValidator(new GetRequestParametersValidator());

            RuleFor(r => r.Id)
                .GreaterThan(0);
        }
    }
}