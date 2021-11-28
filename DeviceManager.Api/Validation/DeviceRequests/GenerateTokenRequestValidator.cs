using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

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