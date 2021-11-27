using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validators
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