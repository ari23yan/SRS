using AutoMapper;
using SurgeryRoomScheduler.Application.Security;
using SurgeryRoomScheduler.Application.Senders;
using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Entities.Common;
using SurgeryRoomScheduler.Domain.Interfaces.Users;
using SurgeryRoomScheduler.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurgeryRoomScheduler.Domain.Dtos.Role;
using SurgeryRoomScheduler.Domain.Entities.General;
using Azure.Core;
using SurgeryRoomScheduler.Application.Utilities;
using SurgeryRoomScheduler.Domain.Dtos.Common;
using SurgeryRoomScheduler.Domain.Dtos.Common.AccessLog;
using SurgeryRoomScheduler.Domain.Enums;
using static System.Net.Mime.MediaTypeNames;
using SurgeryRoomScheduler.Domain.Dtos;

namespace SurgeryRoomScheduler.Application.Services.Implementations
{
    public class TimingService : ITimingService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITimingRepository _timingRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IRepository<UserDetail> _userDetailrepository;
        private readonly IMapper _mapper;
        private readonly ISender _sender;

        public TimingService(IUserRepository userRepository, ISender sender, ITimingRepository timingRepository,
           IRepository<UserDetail> userDetailrepository, IMapper mapper, IDoctorRepository doctorRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userDetailrepository = userDetailrepository;
            _sender = sender;
            _timingRepository = timingRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task<ResponseDto<bool>> CreateTiming(AddTimingDto request, Guid operatorId)
        {
            var checkExist = await _timingRepository.CheckTimingExist(request);
            if (checkExist)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Exist, Status = "Failed", };
            }
            if(request.Date < DateOnly.FromDateTime(DateTime.Now))
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "تاریخ های گذشته را نمیتوان زمانبندی کرد", };
            }
            var rooms = await _doctorRepository.GetDoctorRooms(request.NoNezam);
            if(rooms.Count() < 0)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "اتاقی برای این دکتر تخصیص نیافته است", };
            }
            var check = rooms.Any(x => x.Code == request.RoomCode);
            if(!check)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "پزشک مورد نظر به این اتاق تخصیص داده نشده است" };
            }
            var newTiming = new Timing
            {
                AssignedDoctorNoNezam = request.NoNezam,
                AssignedRoomCode = request.RoomCode,
                ScheduledDate = request.Date,
                ScheduledStartTime = request.StartTime,
                ScheduledEndTime = request.EndTime,
                ScheduledDuration = request.ScheduledDuration,
                CreatedDate_Shamsi = UtilityManager.GregorianDateTimeToPersianDate(DateTime.Now),
                ScheduledDate_Shamsi = UtilityManager.GregorianDateTimeToPersianDateOnly(request.Date),
                CreatedBy = operatorId
            };
            await _timingRepository.AddAsync(newTiming);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
        }

        public async Task<ResponseDto<bool>> DeleteTimingByTimingId(GetByIdDto request, Guid operatorId)
        {
            var timing = await _timingRepository.GetTimingById(request.TargetId);
            if (timing == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "Failed" };
            }
            timing.IsDeleted = true;
            timing.DeletedDate = DateTime.UtcNow;
            timing.DeletedBy = operatorId;
            await _timingRepository.UpdateAsync(timing);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "SuccessFull" };
        }

        public async Task<ResponseDto<IEnumerable<TimingDto>>> GetPaginatedTimingList(PaginationDto request)
        {
            var timings = await _timingRepository.GetPaginatedTimingList(request);
            return new ResponseDto<IEnumerable<TimingDto>>
            {
                IsSuccessFull = true,
                Data = timings.List,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = timings.TotalCount
            };
        }

        public async Task<ResponseDto<IEnumerable<TimingDto>>> GetPaginatedTimingListByRoomAndDate(PaginationDto request, long roomCode, DateOnly date)
        {
            var timings = await _timingRepository.GetPaginatedTimingListByRoomAndDate(request,roomCode,date);
            return new ResponseDto<IEnumerable<TimingDto>>
            {
                IsSuccessFull = true,
                Data = timings.List,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = timings.TotalCount
            };
        }

        public async Task<ResponseDto<GetTimingCalenderDto>> GetTimingCalender(GetListByMonthDto request)
        {
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
            var filtredTiming = await _timingRepository.GetTimingByRoomIdAndDate(request.RoomCode, startDate, endDate);
            calenderDto.Days = new List<DayDto<TimingDto>>();

            foreach (var item in dayInfoDtos)
            {
                var dayDto = new DayDto<TimingDto>
                {
                    Day = item.Day,
                    DayOfTheWeek = item.DayOfTheWeek,
                    Timings = filtredTiming.Where(u => u.ScheduledDate == DateOnly.FromDateTime(item.MiladiDate.Date)).ToList(),
                    IsEnable = item.IsEnable,
                    CountPerDay = filtredTiming.Where(u => u.ScheduledDate == DateOnly.FromDateTime(item.MiladiDate.Date)).Count(),
                    Date = item.ShamsiDate
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
        public async Task<ResponseDto<TimingDto>> GetTimingDetailByTimingId(Guid timingId)
        {
            var timing = await _timingRepository.GetTimingDetailByTimingId(timingId);
            //var mappedTiming = _mapper.Map<Timing, GetTimingDetailDto>(timing);
            return new ResponseDto<TimingDto>
            {
                IsSuccessFull = true,
                Data = timing,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul"
            };
        }

        public async Task<GetExteraTimingDto> GetExteraTimingListByDate(DateOnly date)
        {
           return await _timingRepository.GetExteraTimingListByDate(date);
        }
        public async Task<int> GetTimingsCount()
        {
            return await _timingRepository.GetCountAsync(x => x.IsActive && !x.IsDeleted);
        }

        public async Task<ResponseDto<bool>> UpdateTimingByTimingId(Guid timingId, UpdateTimingDto request, Guid operatorId)
        {
            var timing = await _timingRepository.GetTimingById(timingId);
            if (timing == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Not Found" };
            }
            var mappedTiming = _mapper.Map(request, timing);
            timing.ModifiedBy = operatorId;
            timing.IsModified = true;
            await _timingRepository.UpdateAsync(mappedTiming);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
        }

        public async Task<Timing> GetTimingByTimingId(Guid timingId)
        {
            return await _timingRepository.GetTimingById(timingId);
        }
    }
}
