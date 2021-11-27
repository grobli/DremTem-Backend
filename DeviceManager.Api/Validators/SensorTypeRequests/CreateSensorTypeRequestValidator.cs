using DeviceManager.Core;
using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.SensorTypeRequests
{
    public class CreateSensorTypeRequestValidator : AbstractValidator<CreateSensorTypeRequest>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSensorTypeRequestValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(SensorTypeConfiguration.NameMaxLength)
                .PascalCase()
                .MustAsync(async (name, _) =>
                    await _unitOfWork.SensorTypes.SingleOrDefaultAsync(st => st.Name == name, _) is null)
                .WithMessage("SensorType must have unique Name");

            RuleFor(r => r.DataType)
                .NotEmpty()
                .MaximumLength(SensorTypeConfiguration.DataTypeMaxLength);

            RuleFor(r => r.Unit)
                .NotEmpty()
                .MaximumLength(SensorTypeConfiguration.UnitMaxLength);

            RuleFor(r => r.UnitShort)
                .NotEmpty()
                .MaximumLength(SensorTypeConfiguration.UnitShortMaxLength);

            RuleFor(r => r.UnitSymbol)
                .MaximumLength(SensorTypeConfiguration.UnitSymbolMaxLength);

            RuleFor(r => r.IsDiscrete)
                .NotNull();

            RuleFor(r => r.IsSummable)
                .NotNull();
        }
    }
}