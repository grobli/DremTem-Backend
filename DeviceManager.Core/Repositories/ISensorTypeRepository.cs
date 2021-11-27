using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DeviceManager.Core.Models;
using Shared.Repositories;

namespace DeviceManager.Core.Repositories
{
    public interface ISensorTypeRepository : IRepository<SensorType>
    {
        IQueryable<SensorType> GetSensorTypeById(int typeId);
    }
}