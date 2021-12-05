using FluentValidation;
using Shared.Proto.Common;

namespace DeviceManager.Api.Validation
{
    public class GenericGetManyRequestValidator : AbstractValidator<GenericGetManyRequest>
    {
        public GenericGetManyRequestValidator()
        {
            RuleFor(r => r.Parameters)
                .SetValidator(new GetRequestParametersValidator());

            RuleFor(r => r.PageSize)
                .GreaterThan(0);

            RuleFor(r => r.PageNumber)
                .GreaterThanOrEqualTo(1);
        }
    }
}