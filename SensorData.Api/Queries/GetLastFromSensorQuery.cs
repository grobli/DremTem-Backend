using MediatR;
using Shared.Proto;

namespace SensorData.Api.Queries
{
    public class GetLastFromSensorQuery : IRequest<GetManyFromSensorResponse>
    {
        public GetLastFromSensorQuery(GetLastFromSensorRequest query)
        {
            Query = query;
        }

        public GetLastFromSensorRequest Query { get; }
    }
}