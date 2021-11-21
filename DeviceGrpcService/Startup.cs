using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeviceGrpcService.Data;
using DeviceGrpcService.Models;
using DeviceGrpcService.Proto;
using DeviceGrpcService.Services;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Device = DeviceGrpcService.Models.Device;
using Location = DeviceGrpcService.Models.Location;

namespace DeviceGrpcService
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DeviceContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<DeviceService>();
                endpoints.MapGrpcService<LocationService>();
                endpoints.MapGrpcService<SensorService>();

                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
            });
        }
    }

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Device, DeviceResource>()
                .ForMember(
                    dest => dest.Created,
                    opt =>
                    {
                        opt.PreCondition(src => src.Created.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.Created.Value));
                    })
                .ForMember(
                    dest => dest.LastSeen,
                    opt =>
                    {
                        opt.PreCondition(src => src.LastSeen.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.LastSeen.Value));
                    })
                .ForMember(
                    dest => dest.LastModified,
                    opt =>
                    {
                        opt.PreCondition(src => src.LastModified.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.LastModified.Value));
                    });

            //   .ForMember(dest => dest.Name, opt => opt.NullSubstitute(""));
            CreateMap<Device, DeviceGrpcNestedModel>()
                .ForMember(dest => dest.Name, opt => opt.NullSubstitute(""));
            CreateMap<Location, LocationGrpcModel>();


            // .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
            // .ForMember(dest => dest.LocationID, opt => opt.MapFrom(src => src.LocationID))
            // .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.LocationID))
            // .ForMember(dest => dest.Online, opt => opt.MapFrom(src => src.Online));
        }
    }
}