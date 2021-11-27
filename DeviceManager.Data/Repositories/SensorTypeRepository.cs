using System.Linq;
using System.Threading.Tasks;
using DeviceManager.Core.Models;
using DeviceManager.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Repositories;

namespace DeviceManager.Data.Repositories
{
    public class SensorTypeRepository : Repository<SensorType>, ISensorTypeRepository
    {
        private DeviceManagerContext DeviceManagerContext => Context as DeviceManagerContext;

        public SensorTypeRepository(DeviceManagerContext context) : base(context)
        {
        }

        public IQueryable<SensorType> GetSensorTypeById(int typeId)
        {
            return DeviceManagerContext.SensorTypes
                .Where(st => st.Id == typeId)
                .Take(1);
        }
    }
}