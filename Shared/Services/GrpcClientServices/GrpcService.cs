using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.EntityFrameworkCore.Storage;

namespace Shared.Services.GrpcClientServices
{
    public class GrpcService<TClient> : IGrpcService<TClient> where TClient : ClientBase
    {
        private readonly IGrpcClientProvider<TClient> _clientProvider;

        public GrpcService(IGrpcClientProvider<TClient> clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public TClient GetClient(string serviceId = null) => string.IsNullOrWhiteSpace(serviceId)
            ? _clientProvider.NextClient.client
            : _clientProvider.GetClientById(serviceId);

        public async Task<TResult> SendRequestAsync<TResult>(Func<TClient, Task<TResult>> requestFunc,
            TimeSpan? timeout = null, int retryLimit = 5) where TResult : IMessage, IBufferMessage
        {
            timeout ??= TimeSpan.FromSeconds(10);
            for (var i = 0; i < retryLimit; i++)
            {
                using var tokenSource = new CancellationTokenSource();
                var (client, _) = _clientProvider.NextClient;
                if (client is null) continue;

                var (completed, result) = await ExecuteRequestAsync(requestFunc, timeout.Value, client, tokenSource);
                if (completed) return result;
            }

            throw new RetryLimitExceededException();
        }

        public async Task<TResult> SendRequestAsync<TResult>(Func<TClient, Task<TResult>> requestFunc, string serviceId,
            TimeSpan? timeout = null, int retryLimit = 5) where TResult : IMessage, IBufferMessage
        {
            timeout ??= TimeSpan.FromSeconds(10);
            for (var i = 0; i < retryLimit; i++)
            {
                using var tokenSource = new CancellationTokenSource();
                var client = _clientProvider.GetClientById(serviceId);
                if (client is null) continue;

                var (completed, result) = await ExecuteRequestAsync(requestFunc, timeout.Value, client, tokenSource);
                if (completed) return result;
            }

            throw new RetryLimitExceededException();
        }

        private static async Task<(bool completed, TResult result)> ExecuteRequestAsync<TResult>(
            Func<TClient, Task<TResult>> requestFunc, TimeSpan timeout, TClient client,
            CancellationTokenSource tokenSource)
        {
            var request = Task.Run(() => requestFunc.Invoke(client), tokenSource.Token);
            var delay = Task.Delay(timeout, tokenSource.Token);
            var completedTask = await Task.WhenAny(request, delay);
            tokenSource.Cancel(); // cancel both tasks
            if (completedTask != request) return (false, default);

            // task completed within timeout
            var result = await request;
            return (true, result);
        }
    }
}