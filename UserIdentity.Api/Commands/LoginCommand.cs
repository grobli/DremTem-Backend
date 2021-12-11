using MediatR;
using Shared.Proto;

namespace UserIdentity.Api.Commands
{
    public class LoginCommand : IRequest<UserLoginResponse>
    {
        public LoginCommand(UserLoginRequest body)
        {
            Body = body;
        }

        public UserLoginRequest Body { get; }
    }
}