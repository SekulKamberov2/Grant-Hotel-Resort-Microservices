namespace IdentityServer.Application.Common.Mappings
{
    using AutoMapper;

    using IdentityServer.Domain.Models;
    using IdentityServer.Domain.DTOs;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.Role, opt => opt.Ignore());

            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
        }
    }
}
