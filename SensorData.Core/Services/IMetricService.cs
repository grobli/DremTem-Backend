using System;
using System.Threading.Tasks;
using SensorData.Core.Models;
using Shared;

namespace SensorData.Core.Services
{
    public interface IMetricService
    {
        Task<PagedList<MetricBase>> GetMetricsByRange(int sensorId, MetricMode mode, PaginationParameters parameters,
            DateTime startDate, DateTime endDate = default);
    }
}