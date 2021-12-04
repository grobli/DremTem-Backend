using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Shared.Services.GrpcClientServices
{
    public sealed class GrpcClientProvider<TClient> : IGrpcClientProvider<TClient>, IDisposable
        where TClient : ClientBase
    {
        private readonly ILogger<GrpcClientProvider<TClient>> _logger;
        private readonly IConsulClient _consul;
        private readonly GrpcClientProviderConfig<TClient> _config;
        private ConcurrentDictionary<string, GrpcChannel> Cache { get; } = new();
        private LinkedList<ServiceChannel> Channels { get; } = new();
        private LinkedListNode<ServiceChannel> _currentChannelNode;
        private Action<TClient> InitializeConnectionAction { get; }

        public GrpcClientProvider(IConsulClient consul, IOptions<GrpcClientProviderConfig<TClient>> config,
            ILogger<GrpcClientProvider<TClient>> logger)
        {
            _consul = consul;
            _logger = logger;
            _config = config.Value;
            InitializeConnectionAction = _config.InitializeConnectionAction;
        }


        public (GrpcChannel channel, string serviceId) NextChannel
        {
            get
            {
                lock (Channels)
                {
                    if (_currentChannelNode is null)
                    {
                        _currentChannelNode = Channels.First;
                    }
                    else
                    {
                        _currentChannelNode = _currentChannelNode.Next ?? Channels.First;
                    }

                    return (_currentChannelNode?.Value.Channel, _currentChannelNode?.Value.ServiceId);
                }
            }
        }

        public (TClient client, string serviceId) NextClient
        {
            get
            {
                var (channel, serviceId) = NextChannel;
                return channel is null ? (null, serviceId) : (CreateClient(channel), serviceId);
            }
        }

        private async Task<GrpcChannel> AddChannel(AgentService service)
        {
            if (Cache.TryGetValue(service.ID, out var channel)) return channel;

            channel = GrpcChannel.ForAddress(service.GetAddress());
            Cache.TryAdd(service.ID, channel);
            lock (Channels)
            {
                Channels.AddLast(new ServiceChannel { ServiceId = service.ID, Channel = channel });
            }

            _logger.LogInformation("[{client}] Added new channel: {channel} | Total registered channels: {count}",
                typeof(TClient).Name, channel.Target, Cache.Count);

            await WarmupConnection(channel);
            return channel;
        }

        private static async Task WarmupConnection(GrpcChannel channel)
        {
            var serviceMethods = typeof(TClient)
                .GetMethods()
                .Where(m => m.ReturnType.GetInterfaces().Contains(typeof(IMessage)) ||
                            m.ReturnType.IsGenericType &&
                            m.ReturnType.GenericTypeArguments.First().GetInterfaces().Contains(typeof(IMessage)));
            var client = CreateClient(channel);
            var warmupRequests = new List<Task>();
            foreach (var methodInfo in serviceMethods)
            {
                var arguments = methodInfo.GetParameters()
                    .Select((p, i) => i == 0 ? p.ParameterType : Type.Missing)
                    .ToArray();
                arguments[0] = Activator.CreateInstance((Type)arguments[0])!;
                warmupRequests.Add(Task.Run(() => methodInfo.Invoke(client, arguments)));
            }

            try
            {
                await Task.WhenAll(warmupRequests);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void RemoveChannel(string serviceId)
        {
            Cache.Remove(serviceId, out _);
            lock (Channels)
            {
                var channelNode = Channels.FirstOrDefault(s => s.ServiceId == serviceId);
                if (channelNode is not null) Channels.Remove(channelNode);
            }

            _logger.LogInformation(
                $"[{typeof(TClient).Name}] Removed channel for service: {_config.ServiceName}:{serviceId}");
        }

        public async Task RefreshChannels(CancellationToken token = default)
        {
            var services = await FetchHealthyServiceIds(token);

            var addChannelTasks = services
                .Where(agentService => !Cache.TryGetValue(agentService.ID, out _))
                .Select(AddChannel)
                .ToList();

            await Task.WhenAll(addChannelTasks);
            var addedChannels = addChannelTasks.Select(task => task.Result);

            if (InitializeConnectionAction is not null)
            {
                var initializationTasks = addedChannels
                    .Select(ch => Task.Run(() => InitializeConnectionAction.Invoke(CreateClient(ch)), token));
                await Task.WhenAll(initializationTasks);
            }

            // remove channels that are no longer registered in Consul
            var fetchedIdSet = services.Select(s => s.ID).ToHashSet();
            foreach (var (serviceId, _) in Cache)
            {
                if (!fetchedIdSet.Contains(serviceId)) RemoveChannel(serviceId);
            }
        }

        public TClient GetClientById(string id)
        {
            if (!Cache.TryGetValue(id, out var channel)) return null;
            var grpcClient = CreateClient(channel);
            return grpcClient;
        }

        private async Task<List<AgentService>> FetchHealthyServiceIds(CancellationToken cancellationToken)
        {
            var check = (await _consul.Health.Checks(_config.ServiceName, cancellationToken)).Response;
            RemoveUnhealthyChannels(check);

            var healthyIds = check
                .Where(ch => ch.Status.Equals(HealthStatus.Passing))
                .Select(ch => ch.ServiceID)
                .ToHashSet();
            return (await _consul.Agent.Services(cancellationToken)).Response
                .Where(s => healthyIds.Contains(s.Value.ID))
                .Select(s => s.Value)
                .ToList();
        }

        private void RemoveUnhealthyChannels(IEnumerable<HealthCheck> checks)
        {
            foreach (var unhealthyServiceId in checks
                .Where(ch => !ch.Status.Equals(HealthStatus.Passing))
                .Select(ch => ch.ServiceID))
            {
                if (Cache.ContainsKey(unhealthyServiceId)) RemoveChannel(unhealthyServiceId);
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

        private static TClient CreateClient(GrpcChannel channel)
        {
            return (TClient)Activator.CreateInstance(typeof(TClient), channel);
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
        public Action<T> InitializeConnectionAction { get; set; }
    }
}