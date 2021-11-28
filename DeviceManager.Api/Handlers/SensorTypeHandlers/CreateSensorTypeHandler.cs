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

namespace DeviceManager.Api.Handlers.SensorTypeHandlers
{
    public class CreateSensorTypeHandler : IRequestHandler<CreateSensorTypeCommand, SensorTypeDto>
    {
        private readonly ISensorTypeService _typeService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateSensorTypeRequest> _validator;

        public CreateSensorTypeHandler(ISensorTypeService typeService, IMapper mapper,
            IValidator<CreateSensorTypeRequest> validator)
        {
            _typeService = typeService;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<SensorTypeDto> Handle(CreateSensorTypeCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var newType = _mapper.Map<CreateSensorTypeRequest, SensorType>(request.Body);

            var createdType = await _typeService.CreateSensorTypeAsync(newType, cancellationToken);

            return _mapper.Map<SensorType, SensorTypeDto>(createdType);
        }
    }
}