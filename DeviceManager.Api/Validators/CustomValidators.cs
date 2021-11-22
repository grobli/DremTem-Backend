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
                .WithMessage("UserId must be a valid GUID string");
        }
        
        
    }
}