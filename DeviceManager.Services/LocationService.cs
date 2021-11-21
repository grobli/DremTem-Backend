using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<Location>> GetAllLocations()
        {
            return await _unitOfWork.Locations.GetAllAsync();
        }

        public async Task<IEnumerable<Location>> GetAllLocationsWithDevices()
        {
            return await _unitOfWork.Locations.GetAllWithDevicesAsync();
        }

        public async Task<IEnumerable<Location>> GetAllLocationsOfUser(Guid userId)
        {
            return await _unitOfWork.Locations.GetAllAsync(userId);
        }

        public async Task<IEnumerable<Location>> GetAllLocationsOfUserWithDevices(Guid userId)
        {
            return await _unitOfWork.Locations.GetAllWithDevicesAsync(userId);
        }

        public async Task<Location> GetLocation(Guid userId, string locationName)
        {
            return await _unitOfWork.Locations.GetByIdAsync(userId, locationName);
        }

        public async Task<Location> GetLocationWithDevices(Guid userId, string locationName)
        {
            return await _unitOfWork.Locations.GetWithDevicesByIdAsync(userId, locationName);
        }

        public async Task<Location> CreateLocation(Location newLocation)
        {
            newLocation.Created = DateTime.UtcNow;

            await _unitOfWork.Locations.AddAsync(newLocation);
            await _unitOfWork.CommitAsync();

            return newLocation;
        }

        public async Task UpdateLocation(Location locationToBeUpdated, Location location)
        {
            locationToBeUpdated.LastModified = DateTime.UtcNow;
            locationToBeUpdated.Latitude = location.Latitude;
            locationToBeUpdated.Longitude = location.Longitude;
            locationToBeUpdated.DisplayName = location.DisplayName;

            await _unitOfWork.CommitAsync();
        }

        public async Task DeleteLocation(Location location)
        {
            _unitOfWork.Locations.Remove(location);
            await _unitOfWork.CommitAsync();
        }
    }
}