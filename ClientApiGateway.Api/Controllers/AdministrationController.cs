using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public class AdministrationController : ControllerBase
    {
        private readonly ILogger<AdministrationController> _logger;
        private readonly IEnumerable<EndpointDataSource> _endpointDataSources;
        public static bool IsApiWarmedUp { get; private set; }

        public AdministrationController(ILogger<AdministrationController> logger,
            IEnumerable<EndpointDataSource> endpointDataSources)
        {
            _logger = logger;
            _endpointDataSources = endpointDataSources;
        }

        [Authorize(Roles = DefaultRoles.SuperUser)]
        [HttpGet("warmup")]
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
                var uri = $"{scheme}://localhost:{port}{e.Route}";
                return HitEndpoint(scheme, e.Method, uri, client);
            });

            await Task.WhenAll(warmupTasks);
            IsApiWarmedUp = true;
            return Ok("API is ready :)");
        }

        private async Task HitEndpoint(string scheme, string method, string uri, HttpClient client)
        {
            _logger.LogInformation($"Warming up endpoint: {scheme} {uri}");
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
                _logger.LogDebug($"Error while warming up endpoint: {scheme} {uri}", e);
            }
        }
    }
}