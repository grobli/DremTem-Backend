using System;
using AutoMapper;
using ClientApiGateway.Api.Resources;
using Google.Protobuf.WellKnownTypes;
using Shared.Proto;

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