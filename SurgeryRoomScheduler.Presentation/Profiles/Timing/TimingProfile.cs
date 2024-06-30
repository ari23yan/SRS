using AutoMapper;
using SurgeryRoomScheduler.Domain.Dtos.Role;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Dtos.User;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Entities.General;

namespace SurgeryRoomScheduler.Presentation.Profiles
{
    public class TimingProfile: Profile
    {
        public TimingProfile()
        {

            CreateMap<TimingDto, Timing>().ReverseMap();


            //CreateMap<Timing, TimingListDto>()
            //        .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.AssignedDoctor.FullName))
            //        .ForMember(dest => dest.DoctorNoNezam, opt => opt.MapFrom(src => src.AssignedDoctor.NoNezam))
            //        .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.AssignedRoom.Name));



            //CreateMap<Timing, GetTimingDetailDto>()
            //    .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.AssignedDoctor.FullName))
            //    .ForMember(dest => dest.DoctorNoNezam, opt => opt.MapFrom(src => src.AssignedDoctor.NoNezam))
            //    .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.AssignedRoom.Name));




            CreateMap<UpdateTimingDto, Timing>()
                       .ForMember(dest => dest.AssignedDoctorNoNezam, opt => opt.MapFrom(src => src.NoNezam))
                       .ForMember(dest => dest.AssignedRoomCode, opt => opt.MapFrom(src => src.RoomCode))
                       .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.Now))
                       .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                       .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }

    }
}
