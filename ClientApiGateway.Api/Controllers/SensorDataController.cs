using System;
using System.Net.Mime;
using System.Threading.Tasks;
using ClientApiGateway.Api.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ClientApiGateway.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    [ApiController]
    public class SensorDataController : ControllerBase
    {
        private readonly ILogger<SensorDataController> _logger;

        public SensorDataController(ILogger<SensorDataController> logger)
        {
            _logger = logger;
        }

        // GET api/v1/SensorData/
        [HttpGet]
        public async Task GetAllReadings()
        {
            throw new NotImplementedException();
        }

        // POST api/v1/SensorData/upload/csv
        [HttpPost("upload/csv")]
        public async Task<ActionResult> UploadSensorDataFromCsv(SensorDataCsvResource csvResource)
        {
            return Ok(csvResource);
        }
    }
}