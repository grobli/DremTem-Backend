using AutoMapper;
using UserIdentity.Core.Models.Auth;
using UserIdentity.Core.Proto;

namespace UserIdentity.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserSignUpRequest, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}