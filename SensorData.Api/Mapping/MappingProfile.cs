using System;
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
            CreateMap<DateTime, Timestamp>().ConvertUsing(dt => Timestamp.FromDateTime(dt.ToUniversalTime()));
            CreateMap<Timestamp, DateTime>().ConvertUsing(ts => ts.ToDateTime().ToUniversalTime());

            //---------- Domain to Resource/Request ----------
            CreateMap<Reading, ReadingDto>();

            CreateMap<Reading, ReadingNoSensorDto>();

            CreateMap<Metric, MetricDto>();

            //---------- Resource/Request to Domain ----------
            CreateMap<ReadingDto, Reading>();

            CreateMap<ReadingNoSensorDto, Reading>();

            CreateMap<CreateReadingRequest, Reading>();

            CreateMap<UpdateReadingRequest, Reading>();

            CreateMap<DeleteReadingRequest, Reading>();
        }
    }
}