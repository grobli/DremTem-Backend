﻿using DeviceManager.Core.Proto;
using DeviceManager.Data.Configurations;
using FluentValidation;

namespace DeviceManager.Api.Validators
{
    public class SaveSensorTypeRequestValidator : AbstractValidator<SaveSensorTypeRequest>
    {
        public SaveSensorTypeRequestValidator()
        {
            SetupRules();
        }

        private void SetupRules()
        {
            RuleFor(r => r.Name)
                .NotEmpty()
                .MaximumLength(SensorTypeConfiguration.NameMaxLength);

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