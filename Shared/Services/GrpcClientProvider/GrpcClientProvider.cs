using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Services.GrpcClientProvider
{
    public sealed class GrpcClientProvider<TClient> : IGrpcClientProvider<TClient>, IDisposable
        where TClient : ClientBase
    {
        private readonly IConsulClient _consul;
        private readonly GrpcClientProviderConfig<TClient> _config;
        private Random _random;
        private Random Random => _random ??= new Random();
        private ConcurrentDictionary<string, GrpcChannel> Cache { get; } = new();
        private LinkedList<ServiceChannel> Channels { get; } = new();
        private LinkedListNode<ServiceChannel> _currentChannelNode;

        private ServiceChannel NextChannel
        {
            get
            {
                lock (Channels)
                {
                    if (_currentChannelNode is null)
                    {
                        _currentChannelNode = Channels.First;
                        return _currentChannelNode?.Value;
                    }

                    _currentChannelNode = _currentChannelNode.Next ?? Channels.First;
                    return _currentChannelNode?.Value;
                }
            }
        }

        private GrpcChannel AddChannel(AgentService service)
        {
            if (Cache.TryGetValue(service.ID, out var channel)) return channel;

            channel = GrpcChannel.ForAddress(service.GetAddress());
            Cache.TryAdd(service.ID, channel);
            lock (Channels)
            {
                Channels.AddLast(new ServiceChannel { ServiceId = service.ID, Channel = channel });
            }

            return channel;
        }

        private void RemoveChannel(string serviceId)
        {
            Cache.Remove(serviceId, out _);
            lock (Channels)
            {
                var channelNode = Channels.FirstOrDefault(s => s.ServiceId == serviceId);
                if (channelNode is not null) Channels.Remove(channelNode);
            }
        }

        public GrpcClientProvider(IConsulClient consul, IOptions<GrpcClientProviderConfig<TClient>> config)
        {
            _consul = consul;
            _config = config.Value;
        }

        public async Task<TResult> SendRequestAsync<TResult>(Func<TClient, Task<TResult>> requestFunc,
            TimeSpan? timeout = null, int retryLimit = 5)
        {
            timeout ??= TimeSpan.FromSeconds(10);
            await RefreshChannels();
            for (var i = 0; i < retryLimit; i++)
            {
                using var timeoutCancellationTokenSource = new CancellationTokenSource();

                var channel = NextChannel?.Channel;
                if (channel is null) continue;
                var client = (TClient)Activator.CreateInstance(typeof(TClient), channel);

                var request = Task.Run(() => requestFunc.Invoke(client), timeoutCancellationTokenSource.Token);
                var delay = Task.Delay(timeout.Value, timeoutCancellationTokenSource.Token);
                var completedTask = await Task.WhenAny(request, delay);
                timeoutCancellationTokenSource.Cancel();
                if (completedTask == request)
                {
                    // task completed within timeout
                    return await request;
                }
            }

            throw new RetryLimitExceededException();
        }

        public async Task<TClient> GetRandomClientAsync(CancellationToken cancellationToken = default)
        {
            await RefreshChannels(cancellationToken);
            var chosenService = Cache.ToArray()[Random.Next(Cache.Count)];
            var grpcClient = (TClient)Activator.CreateInstance(typeof(TClient), chosenService.Value);
            return grpcClient;
        }

        private async Task RefreshChannels(CancellationToken cancellationToken = default)
        {
            var services = await FetchHealthyServiceIds(cancellationToken);
            if (services.Count == 0) return;

            foreach (var agentService in services.Where(agentService => !Cache.TryGetValue(agentService.ID, out _)))
            {
                AddChannel(agentService);
            }
        }

        public async Task<TClient> GetClientByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!Cache.TryGetValue(id, out var channel))
            {
                var services = await FetchHealthyServiceIds(cancellationToken);
                var client = services.SingleOrDefault(s => s.ID == id);
                if (client is null) return null;
                channel = GrpcChannel.ForAddress(client.GetAddress());
                Cache.TryAdd(client.ID, channel);
            }

            var grpcClient = (TClient)Activator.CreateInstance(typeof(TClient), channel);
            return grpcClient;
        }

        private async Task<List<AgentService>> FetchHealthyServiceIds(CancellationToken cancellationToken)
        {
            var check = (await _consul.Health.Checks(_config.ServiceName, cancellationToken)).Response;
            RemoveUnhealthyChannelsFromCache(check);

            var healthyIds = check
                .Where(ch => ch.Status.Equals(HealthStatus.Passing))
                .Select(ch => ch.ServiceID)
                .ToHashSet();
            return (await _consul.Agent.Services(cancellationToken)).Response
                .Where(s => healthyIds.Contains(s.Value.ID))
                .Select(s => s.Value)
                .ToList();
        }

        private void RemoveUnhealthyChannelsFromCache(IEnumerable<HealthCheck> checks)
        {
            foreach (var unhealthyServiceId in checks
                .Where(ch => !ch.Status.Equals(HealthStatus.Passing))
                .Select(ch => ch.ServiceID))
            {
                RemoveChannel(unhealthyServiceId);
            }
        }


        public void Dispose()
        {
            _consul?.Dispose();
            foreach (var s in Channels)
            {
                s.Channel.Dispose();
            }
        }
    }

    internal class ServiceChannel
    {
        public string ServiceId { get; set; }
        public GrpcChannel Channel { get; set; }
    }

    internal static class AgentServiceExtension
    {
        public static string GetAddress(this AgentService self)
        {
            return $"{self.Address}:{self.Port}";
        }
    }

    public class GrpcClientProviderConfig<T>
    {
        public string ServiceName { get; set; }
    }

    public static class GrpcServiceProviderExtension
    {
        public static IServiceCollection AddGrpcClientProvider<T>(this IServiceCollection serviceCollection,
            string serviceName) where T : ClientBase
        {
            using var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetRequiredService<IConsulClient>();

            serviceCollection.Configure<GrpcClientProviderConfig<T>>(providerConfig =>
                providerConfig.ServiceName = serviceName);

            serviceCollection.AddSingleton<IGrpcClientProvider<T>, GrpcClientProvider<T>>();

            return serviceCollection;
        }
    }
}