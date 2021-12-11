using FluentValidation;
using Shared.Extensions;
using Shared.Proto;

namespace DeviceManager.Api.Validation
{
    public class GenericDeleteRequestValidator : AbstractValidator<GenericDeleteRequest>
    {
        public GenericDeleteRequestValidator()
        {
            RuleFor(r => r.Id)
                .GreaterThan(0);

            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();
        }
    }
}