using MediatR;
using Shared.Proto;

namespace UserIdentity.Api.Commands
{
    public class DeleteUserCommand : IRequest<DeleteUserResponse>
    {
        public DeleteUserCommand(DeleteUserRequest body)
        {
            Body = body;
        }

        public DeleteUserRequest Body { get; }
    }
}