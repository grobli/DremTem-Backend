using System;
using System.Reflection;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using FluentValidation;
using Grpc.HealthCheck;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SensorData.Api.Consumers;
using SensorData.Api.RpcServices;
using SensorData.Core;
using SensorData.Core.Repositories;
using SensorData.Core.Services;
using SensorData.Core.Settings;
using SensorData.Data;
using SensorData.Data.Repositories;
using SensorData.Services;
using Shared;
using Shared.Extensions;
using Shared.Proto;

namespace SensorData.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostEnvironment environment)
        {
            // setup configuration providers
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddHealthChecks();

            services.AddSingleton<HealthServiceImpl>();

            services.AddHostedService<StatusService>();

            services.AddValidatorsFromAssembly(typeof(Startup).Assembly);

            var dataAssemblyName = typeof(SensorDataContext).Assembly.GetName().Name;
            services.AddDbContext<SensorDataContext>(opt =>
                opt.UseNpgsql(
                        Configuration.GetConnectionString("Default").Replace("Database=data",
                            $"Database=data:{Configuration["User:Id"]}"),
                        x =>
                        {
                            x.MigrationsAssembly(dataAssemblyName);
                            x.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        })
                    .UseSnakeCaseNamingConvention()
            );

            services.AddConsul();
            services.AddGrpcServiceProvider<SensorGrpc.SensorGrpcClient>("DeviceManager");
            services.AddGrpcServiceProvider<SensorTypeGrpc.SensorTypeGrpcClient>("DeviceManager");
            services.AddGrpcServiceProvider<DeviceGrpc.DeviceGrpcClient>("DeviceManager");

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ISensorDataContext, SensorDataContext>();
            services.AddTransient<IReadingService, ReadingService>();
            
            services.AddScoped<IMetricRepository, MetricRepository>();
            services.AddTransient<IMetricService, MetricService>();

            services.AddMediatR(typeof(Startup));

            services.AddAutoMapper(typeof(Startup));

            // messaging
            services.AddSingleton<IBus>(RabbitHutch.CreateBus(Configuration["MessageBroker:ConnectionString"]));
            services.AddSingleton<MessageDispatcher>();
            services.AddSingleton<AutoSubscriber>(provider =>
                new AutoSubscriber(provider.GetRequiredService<IBus>(), "DeviceManager")
                {
                    AutoSubscriberMessageDispatcher = provider.GetRequiredService<MessageDispatcher>()
                });

            // message handlers
            services.AddScoped<UserMessageConsumer>();
            services.AddScoped<DeviceMessageConsumer>();
            services.AddScoped<SensorMessageConsumer>();

            services.Configure<UserSettings>(Configuration.GetSection("User"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SensorDataContext dbContext,
            IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment() || env.IsEnvironment("docker"))
            {
                app.UseDeveloperExceptionPage();
                dbContext.Database.Migrate();
            }

            app.ApplicationServices.GetRequiredService<AutoSubscriber>()
                .SubscribeAsync(Assembly.GetExecutingAssembly().GetTypes());

            app.RegisterWithConsul(lifetime, Configuration["User:Id"]);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<HealthServiceImpl>();
                endpoints.MapGrpcService<SensorDataGrpcService>();

                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. " +
                            "To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
            });
        }
    }
}