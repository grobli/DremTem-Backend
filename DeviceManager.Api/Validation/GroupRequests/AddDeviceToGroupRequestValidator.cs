using DeviceManager.Core;
using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validation.GroupRequests
{
    public class AddDeviceToGroupRequestValidator : AbstractValidator<AddDeviceToGroupRequest>
    {
        public AddDeviceToGroupRequestValidator(IUnitOfWork unitOfWork)
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.DeviceId)
                .NotEmpty();


            RuleFor(r => r.GroupId)
                .NotEmpty();
        }
    }
}