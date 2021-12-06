using MediatR;
using Shared.Proto.SensorData;

namespace SensorData.Api.Queries
{
    public class GetFirstReadingFromSensorQuery : IRequest<ReadingDto>
    {
        public GetFirstReadingFromSensorQuery(GetFirstRecentFromSensorRequest query)
        {
            Query = query;
        }

        public GetFirstRecentFromSensorRequest Query { get; }
    }
}