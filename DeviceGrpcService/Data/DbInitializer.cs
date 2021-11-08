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
                    ID = 1, Name = "Home", Latitude = 54.2, Longitude = 14.34
                }
            };
            foreach (var location in locations)
            {
                context.Locations.Add(location);
            }

            var devices = new Device[]
            {
                new() { LocationID = 1, Name = "Air Temp", Online = true, }
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