using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using UserIdentity.Core.Models.Auth;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IEnumerable<EndpointDataSource> _endpointDataSources;
        public static bool IsApiWarmedUp { get; private set; }

        public AdminController(ILogger<AdminController> logger,
            IEnumerable<EndpointDataSource> endpointDataSources)
        {
            _logger = logger;
            _endpointDataSources = endpointDataSources;
        }

        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpPost("warmup")]
        public async Task<IActionResult> Warmup([FromQuery] bool force)
        {
            if (IsApiWarmedUp && !force) return Ok("API is warmed up already :)");

            var client = new HttpClient(new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
            var endpoints = _endpointDataSources
                .SelectMany(es => es.Endpoints)
                .OfType<RouteEndpoint>()
                .Select(e => new
                {
                    Method = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods[0],
                    Route = $"/{e.RoutePattern.RawText?.TrimStart('/')}"
                })
                .Where(e => !e.Route.EndsWith("/warmup"));

            var scheme = Request.Scheme;
            var port = Request.Host.Port;
            var token = Request.Headers[HeaderNames.Authorization].ToString()
                .Replace("Bearer", "", StringComparison.InvariantCultureIgnoreCase);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var warmupTasks = endpoints.Select(e =>
            {
                var routeWithValues = InsertDummyRouteParameters(e.Route);
                var uri = $"{scheme}://localhost:{port}{routeWithValues}";
                return HitEndpoint(e.Method, uri, client);
            });

            await Task.WhenAll(warmupTasks);
            IsApiWarmedUp = true;
            return Ok("API is ready :)");
        }

        /** replace parameter placeholders with dummy values */
        private static string InsertDummyRouteParameters(string route)
        {
            foreach (Match match in Regex.Matches(route, @"(?=\{)(.*?)(?=\})(\})"))
            {
                var value = match.Value;
                var indexOfColon = value.IndexOf(':') + 1;
                var type = value.Substring(indexOfColon, value.IndexOf('}') - indexOfColon).ToLower();
                route = type switch
                {
                    "int" or "long" => route.Replace(value, "0"),
                    "string" => route.Replace(value, "string"),
                    "guid" => route.Replace(value, Guid.Empty.ToString()),
                    _ => route
                };
            }

            return route;
        }

        private async Task HitEndpoint(string method, string uri, HttpClient client)
        {
            _logger.LogInformation($"Warming up endpoint: {method} {uri}");
            try
            {
                switch (method)
                {
                    case "GET":
                        await client.GetAsync(uri);
                        break;
                    case "POST":
                        await client.PostAsync(uri, null!);
                        break;
                    case "PUT":
                        await client.PutAsync(uri, null!);
                        break;
                    case "DELETE":
                        await client.DeleteAsync(uri);
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error while warming up endpoint: {method} {uri}");
            }
        }
    }
}