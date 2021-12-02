using System;
using System.Collections.Generic;
using DeviceManager.Core.Proto;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Shared.Configs;
using Shared.Extensions;
using Shared.Services;
using Shared.Services.GrpcClientServices;
using UserIdentity.Core.Proto;

namespace ClientApiGateway.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Client Api Gateway", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT containing userid claim",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });

                var security = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            },
                            UnresolvedReference = true
                        },
                        new List<string>()
                    }
                };


                c.AddSecurityRequirement(security);
            });
            services.AddConsul();
            // --------------- gRPC clients ---------------------
            // DeviceManager service
            services.AddGrpcClientProvider<DeviceGrpcService.DeviceGrpcServiceClient>("DeviceManager");
            services.AddGrpcClientProvider<LocationGrpcService.LocationGrpcServiceClient>("DeviceManager");
            services.AddGrpcClientProvider<SensorGrpcService.SensorGrpcServiceClient>("DeviceManager");
            services.AddGrpcClientProvider<SensorTypeGrpcService.SensorTypeGrpcServiceClient>("DeviceManager");

            // UserIdentity service
            services.AddGrpcClientProvider<UserAuthGrpcService.UserAuthGrpcServiceClient>("UserIdentity");
            services.AddGrpcClientProvider<UserGrpcService.UserGrpcServiceClient>("UserIdentity");


            services.AddAutoMapper(typeof(Startup));

            var jwtSettings = Configuration.GetSection("Jwt").Get<JwtConfig>();
            services.AddAuth(jwtSettings);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("docker"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClientApiGateway.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuth();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}