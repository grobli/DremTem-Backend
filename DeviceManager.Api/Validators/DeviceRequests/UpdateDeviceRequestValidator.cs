using DeviceManager.Core;
using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.DeviceRequests
{
    public class UpdateDeviceRequestValidator : AbstractValidator<UpdateDeviceRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDeviceRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.UserId)
                .Guid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));

            // if userId specified then device.userId must match
            Transform(from: r => r, to: r => new { r.Id, r.UserId })
                .MustAsync(async (x, _) =>
                    string.IsNullOrWhiteSpace(x.UserId) ||
                    await _unitOfWork.Devices
                        .SingleOrDefaultAsync(d => d.Id == x.Id && d.UserId.ToString() == x.UserId) is not null
                ).WithMessage("Device not found");
        }
    }
}