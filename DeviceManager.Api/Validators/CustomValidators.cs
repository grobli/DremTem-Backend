using System;
using FluentValidation;

namespace DeviceManager.Api.Validators
{
    public static class CustomValidators
    {
        public static IRuleBuilderOptions<T, string> MustBeValidGuid<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(str => Guid.TryParse(str, out _))
                .WithMessage("{PropertyName} must be a valid GUID string");
        }

        public static IRuleBuilderOptions<T, string> MacAddress<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Matches(
                    @"^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})|([0-9a-fA-F]{4}\\.[0-9a-fA-F]{4}\\.[0-9a-fA-F]{4})$")
                .WithMessage("{PropertyName} must be a valid Mac address");
        }

        public static IRuleBuilderOptions<T, string> PascalCase<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Matches(@"^[A-Z][a-z]+(?:[A-Z][a-z]+)*$")
                .WithMessage("{PropertyName} must be in PascalCase");
        }
    }
}