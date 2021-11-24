using AutoMapper;
using ClientApiGateway.Api.Resources;
using DeviceManager.Core.Proto;
using UserIdentity.Core.Proto;

namespace ClientApiGateway.Api.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateDeviceResource, CreateDeviceRequest>();
            CreateMap<UpdateDeviceResource, UpdateDeviceRequest>();
            CreateMap<UpdateUserDetailsResource, UpdateUserDetailsRequest>();
            CreateMap<CreateLocationResource, CreateLocationRequest>();
            CreateMap<UpdateLocationResource, UpdateLocationRequest>();

            CreateMap<LocationResource, GetLocationResource>();
            CreateMap<LocationResourceExtended, GetLocationResource>();
        }
    }
}