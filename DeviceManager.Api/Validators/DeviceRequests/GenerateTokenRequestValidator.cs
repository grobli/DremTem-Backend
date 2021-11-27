using DeviceManager.Core;
using DeviceManager.Core.Proto;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.DeviceRequests
{
    public class GenerateTokenRequestValidator : AbstractValidator<GenerateTokenRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenerateTokenRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.Id)
                .MustAsync(async (id, _) =>
                    await _unitOfWork.Devices.GetDeviceById(id).SingleOrDefaultAsync(_) is not null)
                .WithMessage("Device with {PropertyName} = \"{PropertyValue}\" not found");
        }
    }
}