using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Repositories
{
    public interface ISensorTypeRepository : IRepository<SensorType>
    {
        Task<SensorType> GetByIdAsync(int typeId);
    }
}