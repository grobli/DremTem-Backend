using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.SensorRequests
{
    public class UpdateSensorRequestValidator : AbstractValidator<UpdateSensorRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSensorRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.UserId)
                .NotEmpty()
                .Guid();

            RuleFor(r => r.DisplayName)
                .MaximumLength(SensorConfiguration.DisplayNameMaxLength);

            RuleFor(r => r.TypeId)
                .NotNull()
                .MustAsync(async (id, _) =>
                    await _unitOfWork.SensorTypes.GetSensorTypeById(id).SingleOrDefaultAsync(_) is not null)
                .WithMessage("SensorType with {PropertyName} = \"{PropertyValue}\" not found.");
        }
    }
}