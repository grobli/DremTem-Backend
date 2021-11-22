using System;
using AutoMapper;
using DeviceManager.Core.Models;
using DeviceManager.Core.Proto;
using Google.Protobuf.WellKnownTypes;

namespace DeviceManager.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //---------- Domain to Resource/Request ----------
            CreateMap<Device, DeviceResource>()
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
                .ForMember(dest => dest.LocationName,
                    opt => opt.Condition(src => !string.IsNullOrEmpty(src.LocationName)))
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => src.UserId.ToString()));

            CreateMap<Sensor, SensorResource>()
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

            CreateMap<SensorType, SensorTypeResource>()
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

            CreateMap<Location, LocationResource>()
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


            //---------- Resource/Request to Domain ----------
            CreateMap<CreateDeviceRequest, Device>()
                .ForMember(
                    dest => dest.UserId,
                    opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(
                    dest => dest.DisplayName,
                    opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.DisplayName)))
                .ForMember(
                    dest => dest.LocationName,
                    opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.LocationName)))
                .ForMember(dest => dest.Sensors, opt => opt.Ignore());

            CreateMap<UpdateDeviceRequest, Device>()
                .ForMember(
                    dest => dest.DisplayName,
                    opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.DisplayName)))
                .ForMember(
                    dest => dest.LocationName,
                    opt => opt.Condition(src => !string.IsNullOrWhiteSpace(src.LocationName)));

            CreateMap<SaveSensorRequest, Sensor>();
            CreateMap<CreateDeviceSensorResource, Sensor>();
            CreateMap<SaveLocationRequest, Location>();
            CreateMap<SaveSensorTypeRequest, SensorType>();
        }
    }
}