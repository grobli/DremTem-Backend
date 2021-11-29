using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Shared.Services.GrpcClientProvider
{
    public class GrpcClientProvider<TClient> : IGrpcClientProvider<TClient>, IDisposable
        where TClient : ClientBase
    {
        private readonly IConsulClient _consul;
        private readonly GrpcClientProviderConfig<TClient> _config;

        private Random _random;
        private Random Random => _random ??= new Random();

        //  private Dictionary<string, ( string id, Uri address)> Cache { get; } = new();
        private Dictionary<string, GrpcChannel> GrpcChannels { get; } = new(); // <service id, grpcChannel>


        public GrpcClientProvider(IConsulClient consul, IOptions<GrpcClientProviderConfig<TClient>> config)
        {
            _consul = consul;
            _config = config.Value;
        }

        public async Task<TClient> GetRandomClientAsync(CancellationToken cancellationToken = default)
        {
            var services = (await _consul.Agent.Services(cancellationToken)).Response;
            var clients = services
                .Where(s => s.Value.Service == _config.ServiceName)
                .Select(s => s.Value)
                .ToList();
            if (clients.Count == 0) return null;

            var client = clients[Random.Next(clients.Count)];
            if (!GrpcChannels.TryGetValue(client.ID, out var channel))
            {
                channel = GrpcChannel.ForAddress(client.GetAddress());
                GrpcChannels.TryAdd(client.ID, channel);
            }

            var grpcClient = (TClient)Activator.CreateInstance(typeof(TClient), channel);
            return grpcClient;
        }


        public async Task<TClient> GetClientByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!GrpcChannels.TryGetValue(id, out var channel))
            {
                var services = (await _consul.Agent.Services(cancellationToken)).Response;
                var client = services
                    .SingleOrDefault(s => s.Value.Service == _config.ServiceName && s.Value.ID == id).Value;
                if (client is null) return null;
                channel = GrpcChannel.ForAddress(client.GetAddress());
                GrpcChannels.TryAdd(client.ID, channel);
            }

            var grpcClient = (TClient)Activator.CreateInstance(typeof(TClient), channel);
            return grpcClient;
        }


        public void Dispose()
        {
            _consul?.Dispose();
            foreach (var (_, channel) in GrpcChannels)
            {
                channel.Dispose();
            }
        }
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