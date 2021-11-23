using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientApiGateway.Api.Extensions;
using DeviceManager.Core.Proto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;
using UserIdentity.Core.Settings;

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

            // --------------- gRPC clients ---------------------
            // DeviceManager services
            var deviceManagerUri = new Uri(Configuration.GetSection("Grpc:Urls")["DeviceManager"]);
            services.AddGrpcClient<DeviceGrpcService.DeviceGrpcServiceClient>(o => o.Address = deviceManagerUri);
            services.AddGrpcClient<LocationGrpcService.LocationGrpcServiceClient>(o => o.Address = deviceManagerUri);
            services.AddGrpcClient<SensorGrpcService.SensorGrpcServiceClient>(o => o.Address = deviceManagerUri);
            services.AddGrpcClient<SensorTypeGrpcService.SensorTypeGrpcServiceClient>(o =>
                o.Address = deviceManagerUri);
            // UserIdentity services
            var userIdentityUri = new Uri(Configuration.GetSection("Grpc:Urls")["UserIdentity"]);
            services.AddGrpcClient<UserAuthGrpcService.UserAuthGrpcServiceClient>(o => o.Address = userIdentityUri);
            services.AddGrpcClient<UserGrpcService.UserGrpcServiceClient>(o => o.Address = userIdentityUri);


            var jwtSettings = Configuration.GetSection("Jwt").Get<JwtSettings>();
            services.AddAuth(jwtSettings);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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