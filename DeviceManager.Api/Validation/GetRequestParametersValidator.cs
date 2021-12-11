using FluentValidation;
using Shared.Extensions;
using Shared.Proto;

namespace DeviceManager.Api.Validation
{
    public class GetRequestParametersValidator : AbstractValidator<GetRequestParameters>
    {
        public GetRequestParametersValidator()
        {
            RuleFor(p => p.UserId)
                .Guid()
                .Unless(p => string.IsNullOrWhiteSpace(p.UserId));
        }
    }
}