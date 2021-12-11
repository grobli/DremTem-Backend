using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using ClientApiGateway.Api.Resources.Device;
using ClientApiGateway.Api.Resources.Group;
using ClientApiGateway.Api.Resources.Location;
using ClientApiGateway.Api.Resources.Reading;
using ClientApiGateway.Api.Resources.Reading.Metric;
using ClientApiGateway.Api.Resources.Sensor;
using ClientApiGateway.Api.Resources.SensorType;
using ClientApiGateway.Api.Resources.User;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Shared.Proto;
using Shared.Proto.Device;
using Shared.Proto.Group;
using Shared.Proto.Location;
using Shared.Proto.Sensor;
using Shared.Proto.SensorData;
using Shared.Proto.SensorType;
using Shared.Proto.User;

namespace ClientApiGateway.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            AllowNullCollections = true;
            AllowNullDestinationValues = true;

            CreateMap<Timestamp, DateTime>().ConvertUsing(ts => ts.ToDateTime().ToUniversalTime());
            CreateMap<DateTime, Timestamp>().ConvertUsing(dt => Timestamp.FromDateTime(dt.ToUniversalTime()));

            // device
            CreateMap<CreateDeviceResource, CreateDeviceRequest>();
            CreateMap<UpdateDeviceResource, UpdateDeviceRequest>();
            CreateMap<DeviceDto, DeviceResource>();
            CreateMap<DeviceExtendedDto, DeviceResource>()
                .ForMember(dest => dest.Sensors,
                    opt => opt.MapFrom(src => src.Sensors.Count > 0 ? src.Sensors : null));

            // location
            CreateMap<CreateLocationResource, CreateLocationRequest>();
            CreateMap<LocationDto, LocationResource>();
            CreateMap<LocationExtendedDto, LocationResource>()
                .ForMember(dest => dest.Devices,
                    opt => opt.MapFrom(src => src.Devices.Count > 0 ? src.Devices : null));
            CreateMap<UpdateLocationResource, UpdateLocationRequest>();

            // sensor
            CreateMap<SaveSensorResource, CreateSensorRequest>();
            CreateMap<SaveSensorResource, UpdateSensorRequest>();
            CreateMap<SensorDto, SensorResource>();

            // sensor type
            CreateMap<UpdateSensorTypeResource, UpdateSensorTypeRequest>();

            // user
            CreateMap<UpdateUserDetailsResource, UpdateUserDetailsRequest>();

            //group 
            CreateMap<GroupDto, GroupResource>();
            CreateMap<GroupTinyDto, GroupResource>();
            CreateMap<CreateGroupResource, CreateGroupRequest>();
            CreateMap<UpdateGroupResource, UpdateGroupRequest>();

            // reading 
            CreateMap<ReadingDto, ReadingResource>();
            CreateMap<ReadingNoSensorDto, ReadingNoSensorResource>();
            CreateMap<GetManyFromSensorResponse, GetReadingsFromSensorResource>();

            CreateMap<MetricDto, MetricNoSensorIdResource>();
        }
    }
}