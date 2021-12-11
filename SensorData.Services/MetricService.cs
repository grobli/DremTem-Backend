using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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


        public async Task<PagedList<Metric>> GetMetricsByRange(int sensorId, MetricMode mode,
            PaginationParameters parameters, DateTime startDate, DateTime endDate = default)
        {
            if (endDate == default) endDate = DateTime.UtcNow;

            var query = _repository
                .GetMetrics(sensorId, mode)
                .Where(m => m.TimeBucket >= startDate && m.TimeBucket <= endDate)
                .OrderByDescending(m => m.TimeBucket);

            return await PagedList<Metric>.ToPagedListAsync(query, parameters.PageNumber, parameters.PageSize);
        }
    }
}