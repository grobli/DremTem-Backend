using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Api.Handlers.SensorTypeHandlers
{
    public class DeleteSensorTypeHandler : IRequestHandler<DeleteSensorTypeCommand, Empty>
    {
        private readonly ISensorTypeService _typeService;
        private readonly IMapper _mapper;
        private readonly IValidator<GenericDeleteRequest> _validator;

        public DeleteSensorTypeHandler(ISensorTypeService typeService, IMapper mapper,
            IValidator<GenericDeleteRequest> validator)
        {
            _typeService = typeService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<Empty> Handle(DeleteSensorTypeCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var type = await _typeService.GetSensorTypeQuery(request.Body.Id)
                .SingleOrDefaultAsync(cancellationToken);
            if (type is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            await _typeService.DeleteSensorTypeAsync(type, cancellationToken);

            return new Empty();
        }
    }
}