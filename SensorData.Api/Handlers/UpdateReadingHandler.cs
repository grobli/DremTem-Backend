using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using SensorData.Api.Commands;
using SensorData.Core.Models;
using SensorData.Core.Services;
using Shared.Proto;

namespace SensorData.Api.Handlers
{
    public class UpdateReadingHandler : IRequestHandler<UpdateReadingCommand, Empty>
    {
        private readonly IReadingService _readingService;
        private readonly IMapper _mapper;

        public UpdateReadingHandler(IReadingService readingService, IMapper mapper)
        {
            _readingService = readingService;
            _mapper = mapper;
        }


        public async Task<Empty> Handle(UpdateReadingCommand command, CancellationToken cancellationToken)
        {
            var readingUpdate = _mapper.Map<UpdateReadingRequest, Reading>(command.Body);
            var readingToUpdate = await _readingService.GetReadingByTimestampAsync(readingUpdate.Time,
                readingUpdate.SensorId, cancellationToken);
            if (readingToUpdate is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Reading not found"));
            }

            await _readingService.UpdateReadingAsync(readingToUpdate, readingUpdate.Value, cancellationToken);
            return new Empty();
        }
    }
}