using FluentValidation;
using Shared.Proto.Common;

namespace DeviceManager.Api.Validation
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