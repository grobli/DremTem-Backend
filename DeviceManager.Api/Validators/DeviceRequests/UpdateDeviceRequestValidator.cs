using System;
using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;
using Grpc.Core;

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
                .MustBeValidGuid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));

            RuleFor(r => r.Id)
                .MustAsync(async (id, _) => await _unitOfWork.Devices.GetByIdAsync(id) is not null)
                .WithMessage("Device with {PropertyName}={PropertyValue} not found");

            RuleFor(r => r.DisplayName)
                .MaximumLength(DeviceConfiguration.DisplayNameMaxLenght);

            // check if location exists and if does then check if it belongs to the user
            Transform(from: r => r, to: r => new { r.LocationId, r.UserId })
                .MustAsync(async (x, _) =>
                {
                    if (x.LocationId is null || string.IsNullOrWhiteSpace(x.UserId)) return true;
                    var location = await _unitOfWork.Locations.GetByIdAsync(x.LocationId.Value);
                    return location is not null && location.UserId.ToString() == x.UserId;
                }).WithMessage("Referenced location not found");

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