using System;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Services.GrpcClientServices
{
    public class GrpcChannelScanner<TClient> : BackgroundService where TClient : ClientBase
    {
        private readonly ILogger<GrpcChannelScanner<TClient>> _logger;
        private readonly IGrpcClientProvider<TClient> _clientProvider;

        public GrpcChannelScanner(IGrpcClientProvider<TClient> clientProvider,
            ILogger<GrpcChannelScanner<TClient>> logger)
        {
            _clientProvider = clientProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _clientProvider.RefreshChannels(stoppingToken);
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error occurred executing {WorkItem}.", nameof(_clientProvider.RefreshChannels));
                }
            }
        }
    }
}