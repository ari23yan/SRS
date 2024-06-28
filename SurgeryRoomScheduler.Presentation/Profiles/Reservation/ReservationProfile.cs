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
            CreateMap<Reservation, AddReservationDto>()
                       .ForMember(dest => dest.DoctorNoNezam, opt => opt.MapFrom(src => src.DoctorNonNezam))
                       .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.RoomCode ?? 0));
        }

    }
}
