using System;
using System.Collections.Generic;
using AutoMapper;
using DeviceGrpcService.Proto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using UserIdentity.Core.Proto;
using WebApiGateway.Extensions;
using WebApiGateway.Settings;

namespace WebApiGateway
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
                .AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true)
                .AddOData(options => options.Select().OrderBy().Filter());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiGateway", Version = "v1" });

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

            // DeviceGrpcService
            services.AddGrpcClient<Device.DeviceClient>(o => { o.Address = new Uri("https://localhost:6001"); });
            services.AddGrpcClient<Location.LocationClient>(o => { o.Address = new Uri("https://localhost:6001"); });
            services.AddGrpcClient<Sensor.SensorClient>(o => { o.Address = new Uri("https://localhost:6001"); });

            services.AddGrpcClient<UserAuthGrpc.UserAuthGrpcClient>(o =>
            {
                o.Address = new Uri("https://localhost:5001");
            });

            services.AddGrpcClient<UserInfoGrpc.UserInfoGrpcClient>(o =>
            {
                o.Address = new Uri("https://localhost:5001");
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiGateway v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuth();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        public class AutoMapperProfile : Profile
        {
            public AutoMapperProfile()
            {
                // CreateMap<DeviceCreateDto, DeviceGrpcBaseModel>()
                //     .ForMember(dest => dest.Name, opt => opt.NullSubstitute(""));


                // .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
                // .ForMember(dest => dest.LocationID, opt => opt.MapFrom(src => src.LocationID))
                // .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.LocationID))
                // .ForMember(dest => dest.Online, opt => opt.MapFrom(src => src.Online));
            }
        }
    }
}