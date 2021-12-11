using AutoMapper;
using Shared.Proto;
using UserIdentity.Core.Models.Auth;

namespace UserIdentity.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //---------- Domain to Resource/Request ----------
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.FirstName,
                    opt => opt.Condition(src => !string.IsNullOrEmpty(src.FirstName)))
                .ForMember(dest => dest.LastName,
                    opt => opt.Condition(src => !string.IsNullOrEmpty(src.LastName)));


            //---------- Resource/Request to Domain ----------

            CreateMap<UserSignUpRequest, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}