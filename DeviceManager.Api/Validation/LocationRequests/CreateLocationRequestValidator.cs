using FluentValidation;
using Shared.Extensions;
using Shared.Proto;

namespace DeviceManager.Api.Validation.LocationRequests
{
    public class CreateLocationRequestValidator : AbstractValidator<CreateLocationRequest>
    {
        public CreateLocationRequestValidator()
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.Name)
                .NotEmpty();
        }
    }
}