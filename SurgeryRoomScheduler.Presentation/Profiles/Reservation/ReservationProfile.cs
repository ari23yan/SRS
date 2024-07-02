using AutoMapper;
using SurgeryRoomScheduler.Domain.Dtos.Reservation;
using SurgeryRoomScheduler.Domain.Dtos.Role;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Entities.General;

namespace SurgeryRoomScheduler.Presentation.Profiles
{
    public class ReservationProfile : Profile
    {
        public ReservationProfile()
        {
            CreateMap<Reservation, AddReservationDto>();
            CreateMap<AddReservationDto, Reservation>()
                
                ;
            //.ForMember(dest => dest.IsConfirmedByMedicalRecords, opt => opt.Ignore())
            //.ForMember(dest => dest.ConfirmedMedicalRecordsUserId, opt => opt.Ignore())
            //.ForMember(dest => dest.IsConfirmedBySupervisor, opt => opt.Ignore())
            //.ForMember(dest => dest.ConfirmedSupervisorUserId, opt => opt.Ignore())
            //.ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<UpdateReservationDto,Reservation>()
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


        }

    }
}
