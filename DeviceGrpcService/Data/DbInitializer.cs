using System;
using System.Linq;
using DeviceGrpcService.Models;

namespace DeviceGrpcService.Data
{
    public static class DbInitializer
    {
        public static void Initialize(DeviceContext context)
        {
            context.Database.EnsureCreated();

            //look for any devices
            if (context.Devices.Any()) return;

            var locations = new Location[]
            {
                new()
                {
                    Id = 1, Name = "Home", Latitude = 54.2, Longitude = 14.34,
                    CreatedById = Guid.Parse("de2fd5b2-fb6e-4698-943a-5e840489fcec"), Created = DateTime.UtcNow
                }
            };
            foreach (var location in locations)
            {
                context.Locations.Add(location);
            }

            var devices = new Device[]
            {
                new()
                {
                    LocationId = 1, Name = "Air Temp", Online = true,
                    OwnerId = Guid.Parse("de2fd5b2-fb6e-4698-943a-5e840489fcec"), ApiKey = "someApikey",
                    Created = DateTime.UtcNow, LastSeen = DateTime.UtcNow.AddDays(1)
                }
            };
            foreach (var dev in devices)
            {
                context.Devices.Add(dev);
            }

            var sensorTypes = new SensorType[]
            {
                new()
                {
                    Name = "Temperature", Unit = "Degree Celsius", DataType = "Numeric", IsDiscrete = false,
                    IsSummable = false, UnitShort = "Degree", UnitSymbol = "℃"
                }
            };
            foreach (var type in sensorTypes)
            {
                context.SensorTypes.Add(type);
            }

            context.SaveChanges();
        }
    }
}