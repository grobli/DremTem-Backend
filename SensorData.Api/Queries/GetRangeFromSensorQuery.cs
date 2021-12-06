using MediatR;
using Shared.Proto.SensorData;

namespace SensorData.Api.Queries
{
    public class GetRangeFromSensorQuery : IRequest<GetManyFromSensorResponse>
    {
        public GetRangeFromSensorQuery(GetRangeFromSensorRequest query)
        {
            Query = query;
        }

        public GetRangeFromSensorRequest Query { get; }
    }
}