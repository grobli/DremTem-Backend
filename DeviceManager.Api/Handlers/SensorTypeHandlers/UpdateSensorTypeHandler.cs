using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DeviceManager.Api.Handlers.SensorTypeHandlers
{
    public class UpdateSensorTypeHandler : IRequestHandler<UpdateSensorTypeCommand, SensorTypeDto>
    {
        private readonly ISensorTypeService _typeService;
        private readonly IMapper _mapper;
        private readonly IValidator<UpdateSensorTypeRequest> _validator;

        public UpdateSensorTypeHandler(ISensorTypeService typeService, IMapper mapper,
            IValidator<UpdateSensorTypeRequest> validator)
        {
            _typeService = typeService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<SensorTypeDto> Handle(UpdateSensorTypeCommand request, CancellationToken cancellationToken)
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

            await _typeService
                .UpdateSensorTypeAsync(type, _mapper.Map<UpdateSensorTypeRequest, SensorType>(request.Body),
                    cancellationToken);

            return await Task.FromResult(_mapper.Map<SensorType, SensorTypeDto>(type));
        }
    }
}