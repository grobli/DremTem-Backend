using DeviceManager.Core;
using DeviceManager.Core.Proto;
using FluentValidation;

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
                .MustBeValidGuid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));

            RuleFor(r => r.Id)
                .MustAsync(async (id, _) => await _unitOfWork.Devices.GetByIdAsync(id) is not null)
                .WithMessage("Device with {PropertyName} = \"{PropertyValue}\" not found");

            // if userId specified then device.userId must match
            Transform(@from: r => r, to: r => new { r.Id, r.UserId })
                .MustAsync(async (x, _) =>
                    string.IsNullOrWhiteSpace(x.UserId) ||
                    await _unitOfWork.Devices
                        .SingleOrDefaultAsync(d => d.Id == x.Id && d.UserId.ToString() == x.UserId) is not null
                ).WithMessage("Device not found");
        }
    }
}