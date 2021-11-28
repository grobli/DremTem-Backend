using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace DeviceManager.Api.Validation.SensorRequests
{
    public class CreateSensorRequestValidator : AbstractValidator<CreateSensorRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSensorRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.Name)
                .NotEmpty();

            RuleFor(r => r.TypeId)
                .NotEmpty()
                .GreaterThan(0);

            RuleFor(r => r.DeviceId)
                .NotEmpty()
                .GreaterThan(0);

            // referenced device must be owned by the user
            Transform(r => r, r => r)
                .MustAsync(async (r, ct) =>
                    await _unitOfWork.Devices.SingleOrDefaultAsync(d =>
                        d.Id == r.DeviceId && d.UserId.ToString() == r.UserId, ct) is not null
                ).WithMessage("referenced device must be owned by the user");
        }
    }
}