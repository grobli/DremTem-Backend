using DeviceManager.Core.Proto;
using FluentValidation;

namespace DeviceManager.Api.Validators.LocationRequests
{
    public class GetAllLocationsRequestValidator : AbstractValidator<GetAllLocationsRequest>
    {
        public GetAllLocationsRequestValidator()
        {
            RuleFor(r => r.UserId)
                .MustBeValidGuid()
                .Unless(r => string.IsNullOrWhiteSpace(r.UserId));
        }   
    }
}