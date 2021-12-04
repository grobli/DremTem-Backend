using System;
using Grpc.Core;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services.GrpcClientServices;

namespace Shared.Extensions
{
    public static class GrpcServiceExtension
    {
        public static IServiceCollection AddGrpcServiceProvider<TClient>(this IServiceCollection serviceCollection,
            string serviceName, Action<TClient> initializeConnectionAction = null) where TClient : ClientBase
        {
            using var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceCollection.Configure<GrpcClientProviderConfig<TClient>>(providerConfig =>
                {
                    providerConfig.ServiceName = serviceName;
                    providerConfig.InitializeConnectionAction = initializeConnectionAction;
                }
            );
            serviceCollection.AddSingleton<IGrpcClientProvider<TClient>, GrpcClientProvider<TClient>>();
            serviceCollection.AddHostedService<GrpcChannelScanner<TClient>>();
            serviceCollection.AddTransient<IGrpcService<TClient>, GrpcService<TClient>>();

            return serviceCollection;
        }
    }
}