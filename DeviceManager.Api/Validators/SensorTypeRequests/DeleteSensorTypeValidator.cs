using DeviceManager.Core;
using DeviceManager.Core.Proto;
using FluentValidation;

namespace DeviceManager.Api.Validators.SensorTypeRequests
{
    public class DeleteSensorTypeValidator : AbstractValidator<GetSensorTypeRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSensorTypeValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.Id)
                .MustAsync(async (id, _) => await _unitOfWork.SensorTypes.GetByIdAsync(id) is not null)
                .WithMessage("SensorType with {PropertyName} = \"{PropertyValue}\" not found");
        }
    }
}