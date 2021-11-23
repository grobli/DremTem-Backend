using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await AddDefaultRolesIfNotExist(host);

            await host.RunAsync();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        private static async Task AddDefaultRolesIfNotExist(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var roleManager = services.GetRequiredService<RoleManager<Role>>();

            if (roleManager.Roles.Any()) return;

            try
            {
                var baseUser = new Role { Name = DefaultRoles.BaseUser };
                var superUser = new Role { Name = DefaultRoles.SuperUser };
                var results = new[]
                {
                    await roleManager.CreateAsync(baseUser),
                    await roleManager.CreateAsync(superUser),
                };
                if (results.Any(r => !r.Succeeded)) throw new Exception();
            }
            catch (Exception e)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(e, "An error occured while adding the default roles. :(");
            }
        }
    }
}