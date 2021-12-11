using FluentValidation;
using Shared.Extensions;
using Shared.Proto;

namespace DeviceManager.Api.Validation.DeviceRequests
{
    public class GenerateTokenRequestValidator : AbstractValidator<GenerateTokenRequest>
    {
        public GenerateTokenRequestValidator()
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();
        }
    }
}