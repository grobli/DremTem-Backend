using System.Linq;
using SensorData.Core.Models;

namespace SensorData.Core.Repositories
{
    public interface IMetricRepository
    {
        IQueryable<Metric> GetMetrics(int sensorId, MetricMode mode);
    }
}