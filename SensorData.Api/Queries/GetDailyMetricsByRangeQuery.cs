using MediatR;
using SensorData.Core.Models;
using Shared.Proto;

namespace SensorData.Api.Queries
{
    public class GetDailyMetricsByRangeQuery : IRequest<GetMetricsByRangeResponse>
    {
        public GetDailyMetricsByRangeQuery(GetMetricsByRangeRequest query, MetricMode mode)
        {
            Query = query;
            MetricMode = mode;
        }

        public GetMetricsByRangeRequest Query { get; }
        public MetricMode MetricMode { get; }
    }
}