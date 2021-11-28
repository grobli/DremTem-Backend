using DeviceManager.Core.Proto;
using MediatR;

namespace DeviceManager.Api.Commands
{
    public class GenerateTokenCommand : IRequest<GenerateTokenResponse>
    {
        public GenerateTokenRequest Body { get; }

        public GenerateTokenCommand(GenerateTokenRequest body)
        {
            Body = body;
        }
    }
}