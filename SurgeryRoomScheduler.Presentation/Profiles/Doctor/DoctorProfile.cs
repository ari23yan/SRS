using AutoMapper;
using Elfie.Serialization;
using SurgeryRoomScheduler.Domain.Dtos;
using SurgeryRoomScheduler.Domain.Dtos;
using SurgeryRoomScheduler.Domain.Dtos;
using SurgeryRoomScheduler.Domain.Dtos.Doctor;
using SurgeryRoomScheduler.Domain.Dtos.Insurance;
using SurgeryRoomScheduler.Domain.Dtos.SurgeryName;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Entities.General;

namespace SurgeryRoomScheduler.Presentation.Profiles
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<SurgeryNameDto, SurgeryName>()
          .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.name_amal))
          .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.Active))
          .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.code));



            CreateMap<RoomDto, Room>()
          .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
          .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
           .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));


            CreateMap<InsuranceDto, Insurance>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name_Bime))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code_Bimeh));


            CreateMap<DoctorRoomDto, DoctorRoom>()
            .ForMember(dest => dest.NoNezam, opt => opt.MapFrom(src => src.NoNezam))
            .ForMember(dest => dest.RoomCode, opt => opt.MapFrom(src => src.RoomId));


            CreateMap<DoctorDto, Doctor>()
            .ForMember(dest => dest.NationalCode, opt => opt.MapFrom(src => src.DoctorNationalCode))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Mobile1))
            .ForMember(dest => dest.CodeFarei, opt => opt.MapFrom(src => src.CodeFarei))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.NameP))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NP))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.FP))
            .ForMember(dest => dest.NationalTariffCode, opt => opt.MapFrom(src => src.NationalTariffCode))
            .ForMember(dest => dest.GeneralSpecialty, opt => opt.MapFrom(src => src.TakhsosKoli))
            .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.Takhsos))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.CodeJ))
            .ForMember(dest => dest.GroupCode, opt => opt.MapFrom(src => src.GroupCode))
            .ForMember(dest => dest.NoNezam, opt => opt.MapFrom(src => src.NoNezam))
            .ForMember(dest => dest.PersonnelCode, opt => opt.MapFrom((src, dest) =>
             {
                 if (string.IsNullOrEmpty(src.Perscode))
                 {
                     return 0;
                 }
                 else
                 {
                     if (string.IsNullOrEmpty(src.Perscode.Trim()))
                     {
                         return 0;
                     }
                     return Convert.ToInt32(src.Perscode.Trim());
                 }
             }))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom((src, dest) =>
            {
                if (src.Active == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }));

            CreateMap<Doctor, DoctorListDto>();

            CreateMap<Room, RoomsListDto>();
        }
    }
}
