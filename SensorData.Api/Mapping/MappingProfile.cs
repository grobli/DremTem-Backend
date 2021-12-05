using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using SensorData.Core.Models;
using Shared.Proto.SensorData;

namespace SensorData.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //---------- Domain to Resource/Request ----------
            CreateMap<Reading, ReadingDto>()
                .ForMember(
                    dest => dest.Time,
                    opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Time)));

            CreateMap<Reading, ReadingNoSensorDto>()
                .ForMember(
                    dest => dest.Time,
                    opt => opt.MapFrom(src => Timestamp.FromDateTime(src.Time)));

            //---------- Resource/Request to Domain ----------
            CreateMap<ReadingDto, Reading>()
                .ForMember(dest => dest.Time,
                    opt => opt.MapFrom(src => src.Time.ToDateTime()));

            CreateMap<ReadingNoSensorDto, Reading>()
                .ForMember(dest => dest.Time,
                    opt => opt.MapFrom(src => src.Time.ToDateTime()));
        }
    }
}