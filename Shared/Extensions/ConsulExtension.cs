using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Configs;

namespace Shared.Extensions
{
    public static class ConsulExtension
    {
        public static IServiceCollection AddConsul(this IServiceCollection serviceCollection)
        {
            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var consulConfig = configuration.GetSection("Consul").Get<ConsulConfig>();

            serviceCollection.Configure<ConsulConfig>(configuration.GetSection("Consul"));
            serviceCollection.AddSingleton<IConsulClient, ConsulClient>(_ => new ConsulClient(cfg =>
                cfg.Address = new Uri(consulConfig.ConsulAddress)
            ));

            return serviceCollection;
        }

        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app,
            IHostApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();
            var consulConfig = app.ApplicationServices.GetRequiredService<IOptions<ConsulConfig>>().Value;

            // setup logger
            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            // get server ip address
            var serverAddressesFeature = app.ServerFeatures.Get<IServerAddressesFeature>();
            var address = serverAddressesFeature.Addresses
                .First()
                .Replace("+", Dns.GetHostEntry(Dns.GetHostName()).AddressList
                    .First(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString());

            // register service with consul
            var uri = new Uri(address);
            var registration = new AgentServiceRegistration
            {
                ID = string.IsNullOrWhiteSpace(consulConfig.ServiceId)
                    ? Guid.NewGuid().ToString()
                    : consulConfig.ServiceId,
                Name = string.IsNullOrWhiteSpace(consulConfig.ServiceName)
                    ? Assembly.GetExecutingAssembly().GetName().Name
                    : consulConfig.ServiceName,
                Address = $"{uri.Scheme}://{uri.Host}",
                Port = uri.Port,
                Check = new AgentCheckRegistration
                {
                    GRPC = $"{uri.Host}:{uri.Port}",
                    GRPCUseTLS = false,
                    Interval = TimeSpan.FromSeconds(10)
                }
            };

            logger.LogInformation($"Registering with Consul as {registration.Name}:{registration.ID}");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Unregistering from Consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;
        }
    }
}