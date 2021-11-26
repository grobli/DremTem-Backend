using DeviceManager.Core;
using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.SensorRequests
{
    public class GetSensorRequestValidator : AbstractValidator<GetSensorRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSensorRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.UserId)
                .Guid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));

            RuleFor(r => r.Id)
                .MustAsync(async (id, _) => await _unitOfWork.Sensors.GetByIdAsync(id) is not null)
                .WithMessage("sensor not found");

            // if userId specified then sensor.userId must match
            Transform(@from: r => r, to: r => new { r.Id, r.UserId })
                .MustAsync(async (x, _) =>
                    string.IsNullOrWhiteSpace(x.UserId) ||
                    (await _unitOfWork.Sensors.GetWithDeviceByIdAsync(x.Id)).Device.UserId.ToString() == x.UserId
                ).WithMessage("sensor not found");
        }
    }
}