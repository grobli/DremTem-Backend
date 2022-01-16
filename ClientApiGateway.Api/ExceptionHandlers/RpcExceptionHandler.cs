using Grpc.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace ClientApiGateway.Api.ExceptionHandlers
{
    public static class RpcExceptionHandler
    {
        public static ActionResult HandleRpcException(RpcException e)
        {
            var message = new
            {
                e.Status.Detail,
                e.Status.StatusCode,
                Status = e.Status.StatusCode.GetDisplayName(),
            };
            return e.Status.StatusCode switch
            {
                StatusCode.NotFound => new NotFoundObjectResult(message),
                StatusCode.PermissionDenied => new UnauthorizedObjectResult(message),
                StatusCode.Unauthenticated => new UnauthorizedObjectResult(message),
                StatusCode.Internal => new StatusCodeResult(StatusCodes.Status500InternalServerError),
                _ => new BadRequestObjectResult(message)
            };
        }
    }
}