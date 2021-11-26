using DeviceManager.Core.Proto;
using FluentValidation;
using Shared.Extensions;

namespace DeviceManager.Api.Validators.LocationRequests
{
    public class GetAllLocationsRequestValidator : AbstractValidator<GetAllLocationsRequest>
    {
        public GetAllLocationsRequestValidator()
        {
            RuleFor(r => r.UserId)
                .Guid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));
        }   
    }
}