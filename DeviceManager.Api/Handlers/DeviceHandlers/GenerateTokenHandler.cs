using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeviceManager.Api.Commands;
using DeviceManager.Core.Services;
using FluentValidation;
using Grpc.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Proto.Device;
using Shared.Extensions;

namespace DeviceManager.Api.Handlers.DeviceHandlers
{
    public class GenerateTokenHandler : IRequestHandler<GenerateTokenCommand, GenerateTokenResponse>
    {
        private readonly IDeviceService _deviceService;
        private readonly IDeviceTokenService _tokenService;
        private readonly IValidator<GenerateTokenRequest> _validator;

        public GenerateTokenHandler(IDeviceService deviceService, IDeviceTokenService tokenService,
            IValidator<GenerateTokenRequest> validator)
        {
            _deviceService = deviceService;
            _tokenService = tokenService;
            _validator = validator;
        }

        public async Task<GenerateTokenResponse> Handle(GenerateTokenCommand request,
            CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request.Body, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, validationResult.Errors.First().ErrorMessage));
            }

            var device = await _deviceService.GetDeviceQuery(request.Body.Id, request.Body.UserId())
                .SingleOrDefaultAsync(cancellationToken);
            if (device is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Not found"));
            }

            return new GenerateTokenResponse
            {
                Id = device.Id,
                Token = await _tokenService.GenerateTokenAsync(device, cancellationToken)
            };
        }
    }
}