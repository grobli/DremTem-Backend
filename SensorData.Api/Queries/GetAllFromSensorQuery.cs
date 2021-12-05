using MediatR;
using Shared.Proto.SensorData;

namespace SensorData.Api.Queries
{
    public class GetAllFromSensorQuery : IRequest<GetManyFromSensorResponse>
    {
        public GetAllFromSensorQuery(GetAllFromSensorRequest query)
        {
            Query = query;
        }

        public GetAllFromSensorRequest Query { get; }
    }
}