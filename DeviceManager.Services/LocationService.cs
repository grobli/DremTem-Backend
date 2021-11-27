using System;
using System.Linq;
using System.Threading.Tasks;
using DeviceManager.Core;
using DeviceManager.Core.Models;
using DeviceManager.Core.Services;

namespace DeviceManager.Services
{
    public class LocationService : ILocationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LocationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IQueryable<Location> GetAllLocations(Guid userId = default)
        {
            return _unitOfWork.Locations.GetLocations(userId);
        }

        public IQueryable<Location> GetLocation(int locationId, Guid userId = default)
        {
            return _unitOfWork.Locations.GetLocationById(locationId, userId);
        }

        public async Task<Location> CreateLocationAsync(Location newLocation)
        {
            newLocation.Created = DateTime.UtcNow;

            await _unitOfWork.Locations.AddAsync(newLocation);
            await _unitOfWork.CommitAsync();

            return newLocation;
        }

        public async Task UpdateLocationAsync(Location locationToBeUpdated, Location location)
        {
            locationToBeUpdated.LastModified = DateTime.UtcNow;

            locationToBeUpdated.Latitude = location.Latitude;
            locationToBeUpdated.Longitude = location.Longitude;
            locationToBeUpdated.DisplayName = location.DisplayName;

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteLocationAsync(Location location)
        {
            _unitOfWork.Locations.Remove(location);
            await _unitOfWork.CommitAsync();
        }
    }
}