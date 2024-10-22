﻿using System;
using System.Reflection;
using DeviceManager.Api.Consumers;
using DeviceManager.Api.RpcServices;
using DeviceManager.Core;
using DeviceManager.Core.Services;
using DeviceManager.Data;
using DeviceManager.Services;
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
using Shared;
using Shared.Configs;
using Shared.Extensions;

namespace DeviceManager.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
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

            var dataAssemblyName = typeof(DeviceManagerContext).Assembly.GetName().Name;
            services.AddDbContext<DeviceManagerContext>(opt =>
                opt.UseNpgsql(Configuration.GetConnectionString("Default"),
                        x =>
                        {
                            x.MigrationsAssembly(dataAssemblyName);
                            x.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                        })
                    .UseSnakeCaseNamingConvention()
            );

            services.AddConsul();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDeviceManagerContext, DeviceManagerContext>();
            services.AddTransient<IDeviceService, DeviceService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<ISensorService, SensorService>();
            services.AddTransient<ISensorTypeService, SensorTypeService>();
            services.AddTransient<IDeviceTokenService, DeviceTokenService>();
            services.AddTransient<IGroupService, GroupService>();
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

            services.Configure<JwtConfig>(Configuration.GetSection("Jwt"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DeviceManagerContext dbContext,
            IBus bus, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment() || env.IsEnvironment("docker"))
            {
                app.UseDeveloperExceptionPage();
                dbContext.Database.Migrate();
            }

            app.ApplicationServices.GetRequiredService<AutoSubscriber>()
                .SubscribeAsync(Assembly.GetExecutingAssembly().GetTypes());

            app.RegisterWithConsul(lifetime);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<HealthServiceImpl>();
                endpoints.MapGrpcService<DeviceGrpcService>();
                endpoints.MapGrpcService<LocationGrpcService>();
                endpoints.MapGrpcService<SensorGrpcService>();
                endpoints.MapGrpcService<SensorTypeGrpcService>();
                endpoints.MapGrpcService<GroupGrpcService>();

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