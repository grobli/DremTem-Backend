using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Data.Configurations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace DeviceManager.Data.Validation
{
    public class DeviceValidator : AbstractValidator<Device>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeviceValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            SetupNameRules();
            SetupDisplayNameRules();
            SetupUserIdRules();
            SetupManufacturerRules();
            SetupModelRules();
            SetupMacAddressRules();
            SetupLocationRules();
        }

        private void SetupNameRules()
        {
            // Device name cannot be null or empty and cannot exceed max length 
            RuleFor(d => d.Name)
                .NotEmpty()
                .MaximumLength(DeviceConfiguration.NameMaxLength);

            // User cannot have two devices with the same name
            Transform(d => d, d => d)
                .MustAsync(async (d, ct) =>
                    await _unitOfWork.Devices.SingleOrDefaultAsync(
                        dev => dev.Id != d.Id && dev.Name == d.Name && dev.UserId == d.UserId, ct) is null)
                .WithMessage("User cannot have two devices with the same name");
        }

        private void SetupUserIdRules()
        {
            RuleFor(d => d.UserId)
                .NotEmpty();
        }

        private void SetupDisplayNameRules()
        {
            RuleFor(d => d.DisplayName)
                .MaximumLength(DeviceConfiguration.DisplayNameMaxLenght);
        }

        private void SetupMacAddressRules()
        {
            RuleFor(d => d.MacAddress)
                .MaximumLength(DeviceConfiguration.MacAddressMaxLength)
                .MacAddress()
                .Unless(d => string.IsNullOrWhiteSpace(d.MacAddress));

            // device mac address must be unique in whole system
            Transform(d => d, d => d)
                .MustAsync(async (d, ct) =>
                    {
                        if (string.IsNullOrWhiteSpace(d.MacAddress)) return true;
                        var device = await _unitOfWork.Devices.SingleOrDefaultAsync(
                            dev => dev.Id != d.Id && dev.MacAddress == d.MacAddress, ct);
                        return device is null;
                    }
                )
                .WithMessage("Device with this mac address is already registered in the system");
        }

        private void SetupModelRules()
        {
            RuleFor(d => d.Model)
                .MaximumLength(DeviceConfiguration.ModelMaxLength);
        }

        private void SetupManufacturerRules()
        {
            RuleFor(d => d.Manufacturer)
                .MaximumLength(DeviceConfiguration.ManufacturerMaxLength);
        }

        private void SetupLocationRules()
        {
            // referenced location must exist and be owned by the same user who owns the device
            Transform(d => d, d => d)
                .MustAsync(async (d, ct) =>
                {
                    if (d.LocationId is null) return true;
                    var location = await _unitOfWork.Locations.GetLocationById(d.LocationId.Value)
                        .SingleOrDefaultAsync(ct);
                    return location is not null && location.UserId == d.UserId;
                }).WithMessage("referenced location must exist and be owned by the same user who owns the device");
        }
    }
}