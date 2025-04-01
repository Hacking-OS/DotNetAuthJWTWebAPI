using AutoMapper;
using DotNetAuth.Domain.Contracts;
using DotNetAuth.Domain.Entities;

namespace DotNetAuth.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
         CreateMap<ApplicationUser, UserResponse>();
         CreateMap<ApplicationUser, CurrentUserResponse>();
         CreateMap<UserRegisterRequest, ApplicationUser>();
        }
    }
}
