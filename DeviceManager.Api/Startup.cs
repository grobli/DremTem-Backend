using DeviceManager.Api.RpcServices;
using DeviceManager.Api.Validators;
using DeviceManager.Core;
using DeviceManager.Core.Services;
using DeviceManager.Core.Services.DeviceTokenService;
using DeviceManager.Data;
using DeviceManager.Services;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

            services.AddValidatorsFromAssemblyContaining<CreateDeviceRequestValidator>();

            var dataAssemblyName = typeof(DeviceManagerContext).Assembly.GetName().Name;
            services.AddDbContext<DeviceManagerContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("Default"),
                    x => x.MigrationsAssembly(dataAssemblyName)));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IDeviceService, DeviceService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<ISensorService, SensorService>();
            services.AddTransient<ISensorTypeService, SensorTypeService>();
            services.AddTransient<IDeviceTokenService, DeviceTokenService>();

            services.AddAutoMapper(typeof(Startup));

            services.Configure<TokenConfig>(Configuration.GetSection("DeviceToken"));
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
                endpoints.MapGrpcService<DeviceGrpcService>();
                endpoints.MapGrpcService<LocationGrpcService>();
                endpoints.MapGrpcService<SensorGrpcService>();
                endpoints.MapGrpcService<SensorTypeGrpcService>();

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