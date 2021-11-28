using System.Linq;
using DeviceManager.Core.Models;
using Shared.Repositories;

namespace DeviceManager.Core.Repositories
{
    public interface ISensorTypeRepository : IRepository<SensorType>
    {
        IQueryable<SensorType> GetSensorTypeById(int typeId);
    }
}