using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Data.Configurations;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Data.Validation
{
    public class SensorTypeValidator : AbstractValidator<SensorType>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SensorTypeValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            SetupRules();
        }

        private void SetupRules()
        {
            SetupNameRules();
            SetupUnitRules();
            SetupUnitShortRules();
            SetupUnitSymbolRules();
            SetupLastModifiedRules();
            SetupCreatedRules();
        }

        private void SetupNameRules()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(SensorTypeConfiguration.NameMaxLength)
                .PascalCase()
                .MustAsync(async (name, ct) =>
                    await _unitOfWork.SensorTypes.SingleOrDefaultAsync(st => st.Name == name, ct) is null)
                .WithMessage("sensor type must have unique name");
        }

        private void SetupUnitRules()
        {
            RuleFor(st => st.Unit)
                .NotEmpty()
                .MaximumLength(SensorTypeConfiguration.UnitMaxLength);
        }

        private void SetupUnitShortRules()
        {
            RuleFor(st => st.UnitShort)
                .NotEmpty()
                .MaximumLength(SensorTypeConfiguration.UnitShortMaxLength);
        }

        private void SetupUnitSymbolRules()
        {
            RuleFor(st => st.UnitSymbol)
                .MaximumLength(SensorTypeConfiguration.UnitSymbolMaxLength);
        }

        private void SetupLastModifiedRules()
        {
            RuleFor(st => st.LastModified)
                .GreaterThanOrEqualTo(st => st.Created)
                .Unless(st => st.LastModified is null);
        }

        private void SetupCreatedRules()
        {
            RuleFor(st => st.Created)
                .NotEmpty();
        }
    }
}