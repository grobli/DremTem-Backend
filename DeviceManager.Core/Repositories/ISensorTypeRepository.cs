using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DeviceManager.Core.Models;

namespace DeviceManager.Core.Repositories
{
    public interface ISensorTypeRepository : IRepository<SensorType>
    {
        [return: MaybeNull]
        Task<SensorType> GetByIdAsync(int typeId);
    }
}