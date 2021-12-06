using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Shared.Configs;
using Shared.Extensions;
using Shared.Proto.Device;
using Shared.Proto.Group;
using Shared.Proto.Location;
using Shared.Proto.Sensor;
using Shared.Proto.SensorData;
using Shared.Proto.SensorType;
using Shared.Proto.User;
using Shared.Proto.UserIdentity;
using Shared.web;

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
            services.AddControllers(options =>
                    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())))
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
            services.AddGrpcServiceProvider<DeviceGrpc.DeviceGrpcClient>("DeviceManager");
            services.AddGrpcServiceProvider<LocationGrpc.LocationGrpcClient>("DeviceManager");
            services.AddGrpcServiceProvider<SensorGrpc.SensorGrpcClient>("DeviceManager");
            services.AddGrpcServiceProvider<SensorTypeGrpc.SensorTypeGrpcClient>("DeviceManager");
            services.AddGrpcServiceProvider<GroupGrpc.GroupGrpcClient>("DeviceManager");

            // UserIdentity service
            services.AddGrpcServiceProvider<UserAuthGrpc.UserAuthGrpcClient>("UserIdentity");
            services.AddGrpcServiceProvider<UserGrpc.UserGrpcClient>("UserIdentity");

            // SensorData service
            services.AddGrpcServiceProvider<SensorDataGrpc.SensorDataGrpcClient>("SensorData");


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