using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

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