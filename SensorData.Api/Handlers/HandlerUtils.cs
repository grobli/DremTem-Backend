using System;
using System.Threading.Tasks;
using Shared.Proto;
using Shared.Services.GrpcClientServices;

namespace SensorData.Api.Handlers
{
    public static class HandlerUtils
    {
        public static async Task<SensorDto> FindSensorByName(IGrpcService<SensorGrpc.SensorGrpcClient> sensorService,
            int deviceId, string sensorName, Guid userId)
        {
            var sensorRequest = new GetSensorByNameRequest
            {
                DeviceId = deviceId,
                SensorName = sensorName,
                Parameters = new GetRequestParameters
                {
                    UserId = userId.ToString()
                }
            };
            var sensorDto = await sensorService.SendRequestAsync(async client =>
                await client.GetSensorByNameAsync(sensorRequest));
    
            return sensorDto;
        }
    }
}