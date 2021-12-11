using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;
using DeviceManager.Data.Validation;
using FluentValidation;

namespace DeviceManager.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IValidator<Location> _validator;
        private IValidator<Location> Validator => _validator ??= new LocationValidator(_unitOfWork);

        public LocationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Location> GetAllLocationsQuery(Guid userId = default)
        {
            return _unitOfWork.Locations.GetLocations(userId);
        }

        public IQueryable<Location> GetLocationQuery(int locationId, Guid userId = default)
        {
            return _unitOfWork.Locations.GetLocationById(locationId, userId);
        }

        public async Task<Location> CreateLocationAsync(Location newLocation,
            CancellationToken cancellationToken = default)
        {
            await Validator.ValidateAndThrowAsync(newLocation, cancellationToken);
            await _unitOfWork.Locations.AddAsync(newLocation, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return newLocation;
        }

        public async Task UpdateLocationAsync(Location locationToBeUpdated, Location location,
            CancellationToken cancellationToken = default)
        {
            var backup = new Location(locationToBeUpdated);

            location.LastModified = DateTime.UtcNow;
            locationToBeUpdated.MapEditableFields(location);

            var validationResult = await Validator.ValidateAsync(locationToBeUpdated, cancellationToken);
            if (!validationResult.IsValid)
            {
                locationToBeUpdated.MapEditableFields(backup);
                throw new ValidationException(validationResult.Errors);
            }

            await _unitOfWork.CommitAsync(cancellationToken);
        }

        public async Task DeleteLocationAsync(Location location, CancellationToken cancellationToken = default)
        {
            _unitOfWork.Locations.Remove(location);
            await _unitOfWork.CommitAsync(cancellationToken);
        }
    }
}