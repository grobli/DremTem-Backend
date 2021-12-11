using System;
using System.Linq;
using SensorData.Core;
using SensorData.Core.Models;
using SensorData.Core.Repositories;

namespace SensorData.Data.Repositories
{
    public class MetricRepository : IMetricRepository
    {
        private readonly ISensorDataContext _context;

        public MetricRepository(ISensorDataContext context)
        {
            _context = context;
        }

        public IQueryable<Metric> GetMetrics(int sensorId, MetricMode mode)
        {
            return mode switch
            {
                MetricMode.Daily => _context.MetricsDaily.Where(m => m.SensorId == sensorId),
                MetricMode.Hourly => _context.MetricsHourly.Where(m => m.SensorId == sensorId),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
        }
    }
}