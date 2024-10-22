﻿using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using EasyNetQ;
using EasyNetQ.AutoSubscribe;
using FluentValidation;
using Grpc.HealthCheck;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Configs;
using Shared.Extensions;
using UserIdentity.Api.Services;
using UserIdentity.Api.Services.Rpc;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Data;

namespace UserIdentity.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();

            services.AddHealthChecks();

            services.AddSingleton<HealthServiceImpl>();

            services.AddHostedService<StatusService>();

            services.AddValidatorsFromAssembly(typeof(Startup).Assembly);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddMediatR(typeof(Startup));

            services.AddConsul();

            // messaging
            services.AddSingleton<IBus>(RabbitHutch.CreateBus(Configuration["MessageBroker:ConnectionString"]));
            services.AddSingleton<MessageDispatcher>();
            services.AddSingleton<AutoSubscriber>(provider =>
                new AutoSubscriber(provider.GetRequiredService<IBus>(), Assembly.GetExecutingAssembly().GetName().Name)
                {
                    AutoSubscriberMessageDispatcher = provider.GetRequiredService<MessageDispatcher>()
                });

            var dataAssemblyName = typeof(UserDbContext).Assembly.GetName().Name;
            services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("Default"),
                    o =>
                    {
                        o.MigrationsAssembly(dataAssemblyName);
                        o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    }));

            services.AddIdentity<User, Role>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Lockout.MaxFailedAccessAttempts = 5;
                })
                .AddEntityFrameworkStores<UserDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<JwtConfig>(Configuration.GetSection("Jwt"));
            services.AddScoped<JwtService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBus bus,
            IHostApplicationLifetime lifetime, UserDbContext dbContext, RoleManager<Role> roleManager,
            UserManager<User> userManager, ILogger<Startup> logger)
        {
            if (env.IsDevelopment() || env.IsEnvironment("docker"))
            {
                app.UseDeveloperExceptionPage();
                dbContext.Database.Migrate();
                AddDefaultRolesIfNotExist(roleManager, logger);
                AddDefaultAdminAccount(userManager, logger);
            }

            // EasyNetQ
            app.ApplicationServices.GetRequiredService<AutoSubscriber>()
                .SubscribeAsync(Assembly.GetExecutingAssembly().GetTypes());

            app.RegisterWithConsul(lifetime);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<HealthServiceImpl>();
                endpoints.MapGrpcService<AuthService>();
                endpoints.MapGrpcService<UserService>();

                endpoints.MapGet("/",
                    async context =>
                    {
                        await context.Response.WriteAsync(
                            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
                    });
            });
        }

        private static void AddDefaultRolesIfNotExist(RoleManager<Role> roleManager,
            ILogger<Startup> logger)
        {
            if (roleManager.Roles.Any()) return;

            try
            {
                var baseUser = new Role { Name = DefaultRoles.BaseUser };
                var superUser = new Role { Name = DefaultRoles.SuperUser };
                Task.WaitAll(roleManager.CreateAsync(baseUser), roleManager.CreateAsync(superUser));
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured while adding the default roles. :(");
            }
        }

        private static void AddDefaultAdminAccount(UserManager<User> userManager, ILogger<Startup> logger)
        {
            if (userManager.Users.Any(user => user.UserName == "admin" && user.Email == "admin")) return;
            Task.WaitAll(CreateAdminAsync());

            async Task CreateAdminAsync()
            {
                var admin = new User { Email = "admin", UserName = "admin" };

                var userCreateResult = await userManager.CreateAsync(admin, "Password@123");
                if (!userCreateResult.Succeeded)
                {
                    const string errorText = "[FATAL] Failed to create default admin account!";
                    logger.LogError(errorText);
                    throw new Exception(errorText);
                }

                var roles = new[] { DefaultRoles.BaseUser, DefaultRoles.SuperUser };
                var roleResult = await userManager.AddToRolesAsync(admin, roles);
                if (!roleResult.Succeeded)
                {
                    await userManager.DeleteAsync(admin);
                    const string errorText = "[FATAL] Failed to add role to default admin account!";
                    logger.LogError(errorText);
                    throw new Exception(errorText);
                }
            }
        }
    }
}