using System;
using System.Linq;
using System.Threading.Tasks;
using SensorData.Core.Models;
using SensorData.Core.Repositories;
using SensorData.Core.Services;
using Shared;

namespace SensorData.Services
{
    public class MetricService : IMetricService
    {
        private readonly IMetricRepository _repository;

        public MetricService(IMetricRepository repository)
        {
            _repository = repository;
        }


        public async Task<PagedList<MetricBase>> GetMetricsByRange(int sensorId, MetricMode mode,
            PaginationParameters parameters, DateTime startDate, DateTime endDate = default)
        {
            if (endDate == default) endDate = DateTime.UtcNow;

            var query = _repository
                .GetMetrics(sensorId, mode)
                .Where(m => m.TimeBucket >= startDate && m.TimeBucket <= endDate)
                .OrderByDescending(m => m.TimeBucket);

            return await PagedList<MetricBase>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
        }
    }
}