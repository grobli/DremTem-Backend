using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using DeviceGrpcService.Proto;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using WebApiGateway.Models;

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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiGateway", Version = "v1" });
            });

            // DeviceGrpcService
            services.AddGrpcClient<Device.DeviceClient>(o =>
            {
                o.Address = new Uri("https://localhost:5005");
                o.ChannelOptionsActions.Add(options =>
                {
                    var clientHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            (_, _, _, _) => true // TODO: Remove in production
                    };

                    options.HttpHandler = clientHandler;
                });
            });
            services.AddGrpcClient<Location.LocationClient>(o =>
            {
                o.Address = new Uri("https://localhost:5005");
                o.ChannelOptionsActions.Add(options =>
                {
                    var clientHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            (_, _, _, _) => true // TODO: Remove in production
                    };

                    options.HttpHandler = clientHandler;
                });
            });
            services.AddGrpcClient<Sensor.SensorClient>(o =>
            {
                o.Address = new Uri("https://localhost:5005");
                o.ChannelOptionsActions.Add(options =>
                {
                    var clientHandler = new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            (_, _, _, _) => true // TODO: Remove in production
                    };

                    options.HttpHandler = clientHandler;
                });
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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

            app.UseAuthorization();

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