using System;
using System.Linq;
using AutoMapper;
using DeviceManager.Core.Messages;
using DeviceManager.Core.Models;
using Google.Protobuf.WellKnownTypes;
using Shared.Proto;
using Shared.Proto.Device;
using Shared.Proto.Group;
using Shared.Proto.Location;
using Shared.Proto.Sensor;
using Shared.Proto.SensorType;

namespace DeviceManager.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //---------- Domain to Resource/Request ----------
            CreateMap<Device, DeviceDto>()
                .ForMember(
                    dest => dest.Created,
                    opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Created)))
                .ForMember(
                    dest => dest.LastSeen,
                    opt =>
                    {
                        opt.PreCondition(src => src.LastSeen.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.LastSeen.Value));
                    })
                .ForMember(
                    dest => dest.LastModified,
                    opt =>
                    {
                        opt.PreCondition(src => src.LastModified.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.LastModified.Value));
                    })
                .ForMember(
                    dest => dest.DisplayName,
                    opt => opt.Condition(src => !string.IsNullOrEmpty(src.DisplayName)))
                .ForMember(dest => dest.LocationId,
                    opt => opt.Condition(src => src.LocationId is not null))
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => src.UserId.ToString()));

            CreateMap<Device, DeviceExtendedDto>()
                .ForMember(
                    dest => dest.Created,
                    opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Created)))
                .ForMember(
                    dest => dest.LastSeen,
                    opt =>
                    {
                        opt.PreCondition(src => src.LastSeen.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.LastSeen.Value));
                    })
                .ForMember(
                    dest => dest.LastModified,
                    opt =>
                    {
                        opt.PreCondition(src => src.LastModified.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.LastModified.Value));
                    })
                .ForMember(
                    dest => dest.DisplayName,
                    opt => opt.Condition(src => !string.IsNullOrEmpty(src.DisplayName)))
                .ForMember(dest => dest.LocationId,
                    opt => opt.Condition(src => src.LocationId is not null))
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => src.UserId.ToString()));

            CreateMap<Sensor, SensorDto>()
                .ForMember(
                    dest => dest.Created,
                    opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Created)))
                .ForMember(
                    dest => dest.LastModified,
                    opt =>
                    {
                        opt.PreCondition(src => src.LastModified.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.LastModified.Value));
                    });

            CreateMap<SensorType, SensorTypeDto>()
                .ForMember(
                    dest => dest.Created,
                    opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Created)))
                .ForMember(
                    dest => dest.LastModified,
                    opt =>
                    {
                        opt.PreCondition(src => src.LastModified.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.LastModified.Value));
                    });

            CreateMap<Location, LocationDto>()
                .ForMember(
                    dest => dest.Created,
                    opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Created)))
                .ForMember(
                    dest => dest.LastModified,
                    opt =>
                    {
                        opt.PreCondition(src => src.LastModified.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.LastModified.Value));
                    });

            CreateMap<Location, LocationExtendedDto>()
                .ForMember(
                    dest => dest.Created,
                    opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Created)))
                .ForMember(
                    dest => dest.LastModified,
                    opt =>
                    {
                        opt.PreCondition(src => src.LastModified.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.LastModified.Value));
                    });

            CreateMap<Device, GenerateTokenResponse>();

            CreateMap<Group, GroupDto>()
                .ForMember(
                    dest => dest.Created,
                    opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Created)))
                .ForMember(
                    dest => dest.LastModified,
                    opt =>
                    {
                        opt.PreCondition(src => src.LastModified.HasValue);
                        opt.MapFrom(src => Timestamp.FromDateTime(src.LastModified.Value));
                    })
                .ForMember(
                    dest => dest.DeviceIds,
                    opt => opt.MapFrom(src => src.Devices.Select(d => d.Id)));


            //---------- Resource/Request to Domain ----------
            CreateMap<CreateDeviceRequest, Device>()
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(
                    dest => dest.DisplayName,
                    opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.DisplayName)))
                .ForMember(
                    dest => dest.LocationId,
                    opt => opt.Condition(src => src.LocationId is not null))
                .ForMember(dest => dest.Sensors, opt => opt.Ignore());

            CreateMap<UpdateDeviceRequest, Device>()
                .ForMember(
                    dest => dest.DisplayName,
                    opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.DisplayName)))
                .ForMember(
                    dest => dest.LocationId,
                    opt => opt.Condition(src => src.LocationId is not null));

            CreateMap<CreateSensorRequest, Sensor>();
            CreateMap<UpdateSensorRequest, Sensor>();
            CreateMap<CreateDeviceSensorResource, Sensor>();
            CreateMap<CreateLocationRequest, Location>();
            CreateMap<UpdateLocationRequest, Location>();
            CreateMap<CreateSensorTypeRequest, SensorType>();
            CreateMap<UpdateSensorTypeRequest, SensorType>();
            CreateMap<CreateGroupRequest, Group>();
            CreateMap<UpdateGroupRequest, Group>();

            // grpc response -> event message
            CreateMap<DeleteDeviceResponse, DeletedDeviceMessage>()
                .ForMember(dest => dest.DeletedSensorIds,
                    opt => opt.MapFrom(src => src.DeletedSensorIds.ToList()));
        }
    }
}