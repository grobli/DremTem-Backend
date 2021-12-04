using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;

namespace Shared.Services.GrpcClientServices
{
    public interface IGrpcService<out TClient> where TClient : ClientBase
    {
        TClient GetClient(string serviceId = null);

        Task<TResult> SendRequestAsync<TResult>(Func<TClient, Task<TResult>> requestFunc, TimeSpan? timeout = null,
            int retryLimit = 5) where TResult : IMessage, IBufferMessage;

        Task<TResult> SendRequestAsync<TResult>(Func<TClient, Task<TResult>> requestFunc, string serviceId,
            TimeSpan? timeout = null,
            int retryLimit = 5) where TResult : IMessage, IBufferMessage;
    }
}