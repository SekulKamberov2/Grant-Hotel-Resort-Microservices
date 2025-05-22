namespace GHR.LeaveManagement.Mapping
{
    using AutoMapper;
    using GHR.LeaveManagement.DTOs.Input;
    using GHR.LeaveManagement.DTOs.Output;
    using GHR.LeaveManagement.Entities;

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateLeaveDto, LeaveApplication>()
                .ForMember(dest => dest.TotalDays, opt => opt.MapFrom(src =>
                    (decimal)(src.EndDate - src.StartDate).TotalDays + 1))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Pending"))
                .ForMember(dest => dest.RequestedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<LeaveApplication, LeaveResponseDto>();
        }
    }

}
