using AutoMapper;
using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Reservation;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Interfaces.Users;
using SurgeryRoomScheduler.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurgeryRoomScheduler.Application.Senders;
using SurgeryRoomScheduler.Data.Repositories;
using SurgeryRoomScheduler.Domain.Entities.General;
using SurgeryRoomScheduler.Application.Utilities;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Dtos.Common;
using SurgeryRoomScheduler.Domain.Dtos;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using Microsoft.IdentityModel.Tokens;

namespace SurgeryRoomScheduler.Application.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ITimingRepository _timingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;
        public ReservationService(IUserRepository userRepository, ISender sender, ITimingRepository timingRepository,
            IReservationRepository reservationRepository, IMapper mapper, IDoctorRepository doctorRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _reservationRepository = reservationRepository;
            _timingRepository = timingRepository;
            _doctorRepository = doctorRepository;
        }

        public Task<ResponseDto<bool>> CancelReservation(GetByIdDto request, Guid operatorId)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<bool>> CreateReservation(AddReservationDto request, Guid currentUser)
        {
            try
            {
                var checkExist = await _reservationRepository.CheckReservationExist(request);
                if (checkExist)
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Exist, Status = "Failed" };
                }
                var timing = await _timingRepository.GetTimingById(request.TimingId);
                if (timing == null)
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "زمان بندی مورد نظر یافت نشد" };
                }
                if (timing.ScheduledDuration <= request.RequestedTime)
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "مدت زمان درخواستی نمیتواند از زمان زمانبندی شده باشد" };
                }
                if(timing.AssignedDoctorNoNezam != request.DoctorNoNezam)
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "زمانبندی مورد نظر برای دکتر دیگری در نظر گرفته شده است" };
                }
                if (timing.AssignedRoomCode != request.RoomCode)
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "زمانبندی مورد نظر برای اتاق عمل دیگری در نظر گرفته شده است" };
                }


                var doctor = await _userRepository.GetUserByUserId(currentUser);
                if (doctor == null)
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Failed" };
                }
                request.DoctorNoNezam = doctor.NoNezam;
                var mappedReservation = _mapper.Map<Reservation>(request);
                await _reservationRepository.AddAsync(mappedReservation);
                return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
            }
            catch (Exception ex)
            {
                throw;
            }
        
        }

        public async Task<ResponseDto<IEnumerable<ReservationDto>>> GetPaginatedReservedList(PaginationDto request, Guid? doctorId)
        {

            var doctor = await _userRepository.GetUserByUserId(doctorId.Value);
            if (doctor == null)
            {
                return new ResponseDto<IEnumerable<ReservationDto>> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Failed" };
            }
            var reservs = await _reservationRepository.GetPaginatedReservedList(request, doctor.NoNezam);
            var reservsCount = await GetReservedCount(doctor.NoNezam);
            //var mappedTimings = _mapper.Map<IEnumerable<Timing>, IEnumerable<TimingListDto>>(timings);
            return new ResponseDto<IEnumerable<ReservationDto>>
            {
                IsSuccessFull = true,
                Data = reservs,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = string.IsNullOrEmpty(request.Searchkey) == true ? reservsCount : reservs.Count()
            };
        }

        public async Task<ResponseDto<IEnumerable<ReservationDto>>> GetPaginatedReservervationsList(PaginationDto request)
        {
            var reservs = await _reservationRepository.GetPaginatedReservervationsList(request);
            var reservsCount = await GetReservedCount(null);
            //var mappedTimings = _mapper.Map<IEnumerable<Timing>, IEnumerable<TimingListDto>>(timings);
            return new ResponseDto<IEnumerable<ReservationDto>>
            {
                IsSuccessFull = true,
                Data = reservs,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = string.IsNullOrEmpty(request.Searchkey) == true ? reservsCount : reservs.Count()
            };
        }

        public async Task<ResponseDto<GetTimingCalenderDto>> GetReservationCalender(GetListByMonthDto request)
        {
            if (!request.UserId.HasValue)
            {
                return new ResponseDto<GetTimingCalenderDto> { IsSuccessFull = false, Message = ErrorsMessages.NotAuthenticated, Status = "Failed" };
            }

            var doctor = await _userRepository.GetUserByUserId(request.UserId.Value);
            if (doctor == null)
            {
                return new ResponseDto<GetTimingCalenderDto> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Failed" };
            }

            DateTime startDate;
            DateTime endDate;
            List<PersianDayInfoDto> dayInfoDtos = new List<PersianDayInfoDto>();
            GetTimingCalenderDto calenderDto = new GetTimingCalenderDto();
            if (request.Month == 0) // currentMonth
            {
                var currentPersianDate = UtilityManager.GregorianDateTimeToPersianDateDashType(DateTime.Now);
                startDate = UtilityManager.ConvertPersianToGregorian(currentPersianDate + "-" + "01");

                var splitDate = currentPersianDate.Split("-");
                var year = int.Parse(splitDate[0]);
                var month = int.Parse(splitDate[1]);
                endDate = UtilityManager.GetLastDayOfPersianMonth(year, month);
                dayInfoDtos = UtilityManager.GetDaysOfPersianMonth(year, month);
                calenderDto.Year = year.ToString();
                calenderDto.Month = month.ToString("D2");
            }
            else
            {
                startDate = UtilityManager.ConvertPersianToGregorian(request.Year + "-" + request.Month + "-" + "01");
                endDate = UtilityManager.GetLastDayOfPersianMonth(request.Year, request.Month);
                dayInfoDtos = UtilityManager.GetDaysOfPersianMonth(request.Year, request.Month);
                calenderDto.Year = request.Year.ToString();
                calenderDto.Month = request.Month.ToString("D2");
            }

            if (request.RoomCode == 0)
            {
                var doctorsRooms = await _doctorRepository.GetDoctorRooms(doctor.NoNezam);
                if (doctorsRooms.Count() <= 0)
                {
                    return new ResponseDto<GetTimingCalenderDto> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "پزشک به هیچ اتاق عملی تخصیص نیافته است  " };
                }
                else
                {
                    request.RoomCode = doctorsRooms.First().Code.Value;
                }
            }
            var filtredTiming = await _timingRepository.GetDoctorTimingByRoomIdAndDate(request.RoomCode, doctor.NoNezam, startDate, endDate);
            calenderDto.Days = new List<DayDto<TimingDto>>();
            foreach (var item in dayInfoDtos)
            {
                var dayDto = new DayDto<TimingDto>
                {
                    Day = item.Day,
                    DayOfTheWeek = item.DayOfTheWeek,
                    Timings = filtredTiming.Where(u => u.ScheduledStartDate.Date == item.MiladiDate.Date).ToList(),
                    IsEnable = item.IsEnable,
                    CountPerDay = filtredTiming.Where(u => u.ScheduledStartDate.Date == item.MiladiDate.Date).Count(),
                    Date = item.ShamsiDate,
                };
                calenderDto.Days.Add(dayDto);
            }
            calenderDto.MonthName = UtilityManager.GetPersianDateWithDetails(startDate.ToString("yyyyMMdd")).PersianMonth;
            return new ResponseDto<GetTimingCalenderDto>
            {
                IsSuccessFull = true,
                Data = calenderDto,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = dayInfoDtos.Count()
            };
        }

        public async Task<int> GetReservedCount(string? noNezam)
        {
            if (!noNezam.IsNullOrEmpty())
            {
                return await _reservationRepository.GetCountAsync(x => x.IsActive && !x.IsDeleted && x.DoctorNoNezam.Equals(noNezam));
            }
            return await _reservationRepository.GetCountAsync(x => x.IsActive && !x.IsDeleted);
        }

        public async Task<ResponseDto<bool>> UpdateReservationByReservationId(Guid reservationId, UpdateReservationDto request, Guid operatorId)
        {
            var reservation = await _reservationRepository.GetReservationById(reservationId);
            if (reservation == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Not Found" };
            }
            var mappedTiming = _mapper.Map(request, reservation);
            reservation.ModifiedBy = operatorId;
            reservation.IsModified = true;
            await _reservationRepository.UpdateAsync(mappedTiming);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
        }
    }
}
