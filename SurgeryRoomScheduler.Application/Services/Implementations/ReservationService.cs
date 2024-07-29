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
using SurgeryRoomScheduler.Domain.Enums;
using Azure.Core;
using Microsoft.OpenApi.Models;
using SurgeryRoomScheduler.Domain.Entities.Common;
using ReservationConfirmationStatus = SurgeryRoomScheduler.Domain.Enums.ReservationConfirmationStatus;
using AutoMapper;
using SurgeryRoomScheduler.Domain.Dtos.Insurance;
using SurgeryRoomScheduler.Domain.Dtos.SurgeryName;

namespace SurgeryRoomScheduler.Application.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ITimingRepository _timingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly ISender _sender;
        private readonly IRepository<ReservationConfirmation> _reservationConfirmation;
        private readonly IRepository<ReservationConfirmationLog> _reservationConfirmationLog;
        private readonly IMapper _mapper;
        public ReservationService(IUserRepository userRepository, ISender sender, ITimingRepository timingRepository,
            IReservationRepository reservationRepository, IMapper mapper, IDoctorRepository doctorRepository
            , IRepository<ReservationConfirmationLog> reservationConfirmationLog,
            IRepository<ReservationConfirmation> reservationConfirmation)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _reservationRepository = reservationRepository;
            _timingRepository = timingRepository;
            _doctorRepository = doctorRepository;
            _reservationConfirmation = reservationConfirmation;
            _reservationConfirmationLog = reservationConfirmationLog;
            _sender = sender;
        }

        public async Task<ResponseDto<bool>> CancelReservation(CancelReservationDto request, Guid operatorId)
        {
            var user = await _userRepository.GetUserWithRolesByUserId(operatorId);
            if (user == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.UserNotfound, Status = "Failed" };
            }
            var reservationConf = await _reservationRepository.GetReservationConfirmationByReservationId(request.ReservationId);
            if (reservationConf == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "درخواست یافت نشد" };
            }
            var reservation = await _reservationRepository.GetReservationById(request.ReservationId);
            if (reservation == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "درخواست یافت نشد" };
            }
            if (reservationConf.IsConfirmedByMedicalRecords)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "امکان کنسل کردن این رکورد وجود ندارد زیرا توسط مدارک پزشکی تایید شده است" };
            }
            reservationConf.StatusId = (int)Domain.Enums.ReservationConfirmationStatus.CancelledByDoctor;
            reservationConf.ModifiedDate = DateTime.Now;
            reservationConf.IsModified = true;
            reservationConf.ModifiedBy = operatorId;
            reservationConf.RejectionAndCancellationUserId = operatorId;
            reservationConf.RejectionAndCancellationAdditionalDescription = request.CancellationDescription;
            reservationConf.ReservationRejectionAndCancellationReasonId = request.ReservationCancellationReasonId;
            await _reservationConfirmation.UpdateAsync(reservationConf);
            reservation.ModifiedDate = DateTime.Now;
            reservation.IsModified = true;
            reservation.ModifiedBy = operatorId;
            await _reservationRepository.UpdateAsync(reservation);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
        }

        public async Task<ResponseDto<bool>> ConfirmReservation(Guid reservationId, Guid operatorId)
        {

            var user = await _userRepository.GetUserWithRolesByUserId(operatorId);
            if (user == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.UserNotfound, Status = "Failed" };
            }
            var reservationConf = await _reservationRepository.GetReservationConfirmationByReservationId(reservationId);
            if (reservationConf == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "درخواست یافت نشد" };
            }
            //if (reservationConf.StatusId != (int)Domain.Enums.ReservationConfirmationStatus.Pending)
            //{
            //    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست قبلا تعیین وضعیت شده است" };
            //}





            ReservationConfirmationLog confirmationLog = new ReservationConfirmationLog
            {
                ConfirmationAction = ConfirmationAction.Approved,
                ReservationConfirmationTypeId = reservationConf.ReservationConfirmationType.Id,
                OperatorId = operatorId,
                ReservationId = reservationId,
            };

            await _reservationConfirmationLog.AddAsync(confirmationLog);




            if (reservationConf.ReservationConfirmationType.Name == "Normal-Reservation") // نوبت های عادی
            {
                if (user.Role.RoleName == "MedicalRecord")
                {
                    if (reservationConf.IsConfirmedByMedicalRecords)
                    {
                        return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست قبلا توسط مدارک پزشکی تعیین وضعیت شده است" };
                    }
                    else
                    {
                        // Approved
                        reservationConf.StatusId = (int)Domain.Enums.ReservationConfirmationStatus.ApprovedByMedicalRecord;
                        reservationConf.ConfirmedMedicalRecordsUserId = operatorId;
                        reservationConf.IsConfirmedByMedicalRecords = true;
                        await _reservationConfirmation.UpdateAsync(reservationConf);
                        return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
                    }
                }
                else
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "کاربر مدارک پزشکی فقط امکان تایید نوبت های عادی را دارد" };
                }
            }
            else if (reservationConf.ReservationConfirmationType.Name == "Extera-Reservation") // نوبت های مازاد
            {
                if (user.Role.RoleName == "Supervisor")
                {
                    if (reservationConf.IsConfirmedBySupervisor)
                    {
                        return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست قبلا توسط سوپروایزر تعیین وضعیت شده است" };
                    }
                    else
                    {
                        // Approved
                        reservationConf.StatusId = (int)Domain.Enums.ReservationConfirmationStatus.ApprovedBySupervisor;
                        reservationConf.ConfirmedSupervisorUserId = operatorId;
                        reservationConf.IsConfirmedBySupervisor = true;
                        await _reservationConfirmation.UpdateAsync(reservationConf);
                        return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
                    }
                }
                else if (user.Role.RoleName == "MedicalRecord")
                {
                    if (!reservationConf.IsConfirmedBySupervisor)
                    {
                        return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست اول باید توسط  سوپروایزر تایید  شود " };
                    }
                    if (reservationConf.IsConfirmedByMedicalRecords)
                    {
                        return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست قبلا توسط مدارک پزشکی تعیین وضعیت شده است" };
                    }
                    else
                    {
                        //Approved
                        reservationConf.StatusId = (int)Domain.Enums.ReservationConfirmationStatus.ApprovedByMedicalRecord;
                        reservationConf.ConfirmedMedicalRecordsUserId = operatorId;
                        reservationConf.IsConfirmedByMedicalRecords = true;
                        await _reservationConfirmation.UpdateAsync(reservationConf);
                        return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
                    }
                }
            }
            return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.PermissionDenied, Status = "Failed" };
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
                var doctor = await _userRepository.GetUserByUserId(currentUser);
                if (doctor == null)
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Failed" };
                }
                if (request.RequestedTime > timing.ScheduledDuration)
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "مدت زمان انتخاب شده بیشتر  از حد مجاز است." };
                }
                if (timing.IsExtraTiming)
                {
                    request.DoctorNoNezam = request.DoctorNoNezam;
                }
                else
                {
                    request.DoctorNoNezam = timing.AssignedDoctorNoNezam;
                    if (timing.AssignedDoctorNoNezam != request.DoctorNoNezam)
                    {
                        return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "زمانبندی مورد نظر برای دکتر دیگری در نظر گرفته شده است" };
                    }
                }
                request.RoomCode = timing.AssignedRoomCode;

                var reservation = _mapper.Map<Reservation>(request);
                reservation.UsageTime = timing.ScheduledDuration - request.RequestedTime;
                reservation.RequestedDate = timing.ScheduledDate.ToDateTime(TimeOnly.Parse("00:00:00 PM")); ;
                await _reservationRepository.AddAsync(reservation);
                var reservationConfirmation = new ReservationConfirmation
                {
                    ReservationId = reservation.Id,
                    ReservationConfirmationTypeId = new Guid("c25c174c-efd0-4a69-8207-a48fe437268b") // pending
                };
                await _reservationConfirmation.AddAsync(reservationConfirmation);
                reservation.ReservationConfirmationId = reservationConfirmation.Id;
                await _reservationRepository.UpdateAsync(reservation);
                return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<ResponseDto<IEnumerable<TimingDto>>> GetExteraTimingsList(PaginationDto request, Guid? userId, long roomCode)
        {
            var user = await _userRepository.GetUserWithRolesByUserId(userId.Value);
            if (user == null)
            {
                return new ResponseDto<IEnumerable<TimingDto>> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Failed" };
            }
            if (!user.Role.RoleName.Equals("NormalUser"))
            {
                roomCode = 00;
            }
            else if (roomCode == 0)
            {
                var doctorsRooms = await _doctorRepository.GetDoctorRooms(user.NoNezam);
                if (doctorsRooms.Count() <= 0)
                {
                    return new ResponseDto<IEnumerable<TimingDto>> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "پزشک به هیچ اتاق عملی تخصیص نیافته است  " };
                }
                else
                {
                    roomCode = doctorsRooms.First().Code.Value;
                }
            }
            var filtredTiming = await _timingRepository.GetExteraTimingListByRoomCode(request, roomCode);

            return new ResponseDto<IEnumerable<TimingDto>>
            {
                IsSuccessFull = true,
                Data = filtredTiming.List,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = filtredTiming.TotalCount
            };
        }

        public async Task<ResponseDto<IEnumerable<ReservationDto>>> GetPaginatedReservedList(PaginationDto request, Guid? doctorId, bool isExtera)
        {

            var doctor = await _userRepository.GetUserByUserId(doctorId.Value);
            if (doctor == null)
            {
                return new ResponseDto<IEnumerable<ReservationDto>> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Failed" };
            }
            var reservs = await _reservationRepository.GetPaginatedReservedList(request, doctor.NoNezam, isExtera);
            return new ResponseDto<IEnumerable<ReservationDto>>
            {
                IsSuccessFull = true,
                Data = reservs.List,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = reservs.TotalCount
            };
        }



        public async Task<ResponseDto<IEnumerable<ReservationDto>>> GetExteraReservationList(PaginationDto request, Guid? operatorId)
        {

            var user = await _userRepository.GetUserByUserId(operatorId.Value);
            if (user == null)
            {
                return new ResponseDto<IEnumerable<ReservationDto>> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Failed" };
            }
            var reservs = await _reservationRepository.GetExteraReservationList(request);
            //var mappedTimings = _mapper.Map<IEnumerable<Timing>, IEnumerable<TimingListDto>>(timings);
            return new ResponseDto<IEnumerable<ReservationDto>>
            {
                IsSuccessFull = true,
                Data = reservs.List,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = reservs.TotalCount
            };
        }




        public async Task<ResponseDto<IEnumerable<ReservationDto>>> GetReservationRejectionList(PaginationDto request, Guid? doctorId)
        {

            var doctor = await _userRepository.GetUserByUserId(doctorId.Value);
            if (doctor == null)
            {
                return new ResponseDto<IEnumerable<ReservationDto>> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Failed" };
            }
            var reservs = await _reservationRepository.GetReservationRejectionList(request, doctor.NoNezam);
            //var mappedTimings = _mapper.Map<IEnumerable<Timing>, IEnumerable<TimingListDto>>(timings);
            return new ResponseDto<IEnumerable<ReservationDto>>
            {
                IsSuccessFull = true,
                Data = reservs.List,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = reservs.TotalCount
            };
        }

        public async Task<ResponseDto<IEnumerable<ReservationDto>>> GetPaginatedReservervationsList(PaginationDto request, Guid operatorId, ReservationStatus status)
        {
            var user = await _userRepository.GetUserWithRolesByUserId(operatorId);
            if (user == null)
            {
                return new ResponseDto<IEnumerable<ReservationDto>> { IsSuccessFull = false, Message = ErrorsMessages.UserNotfound, Status = "Failed" };
            }
            var reservs = await _reservationRepository.GetPaginatedReservervationsList(request, user.Role.RoleName, status);

            return new ResponseDto<IEnumerable<ReservationDto>>
            {
                IsSuccessFull = true,
                Data = reservs.List,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = reservs.TotalCount
            };
        }

        public async Task<ResponseDto<IEnumerable<ReservationRejectionAndCancellationReason>>> GetRejectionsReasons(Guid operatorId, bool isCancellation)
        {
            var user = await _userRepository.GetUserWithRolesByUserId(operatorId);
            if (user == null)
            {
                return new ResponseDto<IEnumerable<ReservationRejectionAndCancellationReason>> { IsSuccessFull = false, Message = ErrorsMessages.UserNotfound, Status = "Failed" };
            }
            IEnumerable<ReservationRejectionAndCancellationReason> records = new List<ReservationRejectionAndCancellationReason>();

            if (isCancellation)
            {
                records = await GetReservationRejectionReasonByType(RejectionReasonType.cancellation);
            }
            else
            {
                if (user.Role.RoleName == "Supervisor")
                {
                    records = await GetReservationRejectionReasonByType(RejectionReasonType.Supervisor);
                }
                else if (user.Role.RoleName == "MedicalRecord")
                {
                    records = await GetReservationRejectionReasonByType(RejectionReasonType.MedicalRecords);
                }
            }

            return new ResponseDto<IEnumerable<ReservationRejectionAndCancellationReason>>
            {
                IsSuccessFull = true,
                Data = records,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
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
                dayInfoDtos = UtilityManager.GetDaysOfPersianMonthForDoctorsCalender(request.Year, request.Month);
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
            var filtredReservation = await _reservationRepository.GetDoctorReservationByRoomIdAndDate(request.RoomCode, doctor.NoNezam, startDate, endDate);
            calenderDto.Days = new List<DayDto<TimingDto>>();
            foreach (var item in dayInfoDtos)
            {
                var dayDto = new DayDto<TimingDto>
                {
                    Day = item.Day,
                    DayOfTheWeek = item.DayOfTheWeek,
                    Timings = filtredTiming.Where(u => u.ScheduledDate == DateOnly.FromDateTime(item.MiladiDate.Date)).ToList(),
                    Reservations = filtredReservation.Where(u => u.RequestedDate.Date == item.MiladiDate.Date).ToList(),
                    IsEnable = item.IsEnable,
                    CountPerDay = filtredTiming.Where(u => u.ScheduledDate == DateOnly.FromDateTime(item.MiladiDate.Date)).Count(),
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

        public async Task<IEnumerable<ReservationRejectionAndCancellationReason>> GetReservationRejectionReasonByType(RejectionReasonType type)
        {
            return await _reservationRepository.GetReservationRejectionReasonByType(type);
        }

        public async Task<int> GetReservedConfirmationCountByType(string? type, ReservationStatus status)
        {

            if (type == "Supervisor")
            {
                if (status == ReservationStatus.Approved)
                {
                    return await _reservationConfirmation.GetCountAsync(x => x.IsActive && !x.IsDeleted && x.IsConfirmedBySupervisor);
                }
                else if (status == ReservationStatus.Rejected)
                {
                    return await _reservationConfirmation.GetCountAsync(x => x.IsActive && !x.IsDeleted && !x.IsConfirmedBySupervisor);
                }
                else if (status == ReservationStatus.Pending)
                {
                    return await _reservationConfirmation.GetCountAsync(x => x.IsActive && !x.IsDeleted && x.ConfirmedSupervisorUserId == null);
                }
            }
            else if (type == "MedicalRecord")
            {
                if (status == ReservationStatus.Approved)
                {
                    return await _reservationConfirmation.GetCountAsync(x => x.IsActive && !x.IsDeleted && x.IsConfirmedByMedicalRecords);
                }
                else if (status == ReservationStatus.Rejected)
                {
                    return await _reservationConfirmation.GetCountAsync(x => x.IsActive && !x.IsDeleted && !x.IsConfirmedByMedicalRecords);
                }
                else if (status == ReservationStatus.Pending)
                {
                    return await _reservationConfirmation.GetCountAsync(x => x.IsActive && !x.IsDeleted && x.ConfirmedMedicalRecordsUserId == null);
                }
            }
            return await _reservationRepository.GetCountAsync(x => x.IsActive && !x.IsDeleted);
        }
        public async Task<int> GetReservedCount(string? noNezam)
        {
            if (!noNezam.IsNullOrEmpty())
            {
                return await _reservationRepository.GetCountAsync(x => x.IsActive && !x.IsDeleted && x.DoctorNoNezam.Equals(noNezam));
            }
            return await _reservationRepository.GetCountAsync(x => x.IsActive && !x.IsDeleted);
        }




        public async Task<int> GetExteraReservedCount(long roomCode)
        {
            return await _timingRepository.GetCountAsync(x => x.IsActive && !x.IsDeleted && x.IsExtraTiming && x.AssignedRoomCode == roomCode && x.ScheduledDate >= DateOnly.FromDateTime(DateTime.Now) && x.ScheduledDate <= DateOnly.FromDateTime(DateTime.Now.AddDays(3)));
        }

        public async Task<ResponseDto<bool>> RejectReservationRequest(RejectReservationRequestDto request, Guid operatorId)
        {
            var user = await _userRepository.GetUserWithRolesByUserId(operatorId);
            if (user == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.UserNotfound, Status = "Failed" };
            }
            var reservationConf = await _reservationRepository.GetReservationConfirmationByReservationId(request.ReservationId);
            if (reservationConf == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "درخواست یافت نشد" };
            }
            if (reservationConf.StatusId != (int)Domain.Enums.ReservationConfirmationStatus.Pending)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست قبلا تعیین وضعیت شده است" };
            }

            ReservationConfirmationLog confirmationLog = new ReservationConfirmationLog
            {
                ConfirmationAction = ConfirmationAction.Rejected,
                ReservationConfirmationTypeId = reservationConf.ReservationConfirmationType.Id,
                OperatorId = operatorId,
                ReservationId = request.ReservationId,
            };

            await _reservationConfirmationLog.AddAsync(confirmationLog);


            if (reservationConf.ReservationConfirmationType.Name == "Normal-Reservation") // نوبت های عادی
            {
                if (user.Role.RoleName == "MedicalRecord")
                {
                    if (reservationConf.IsConfirmedByMedicalRecords)
                    {
                        return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست قبلا توسط  مدارک پزشکی تعیین وضعیت شده است" };
                    }
                    else
                    {

                        reservationConf.StatusId = (int)Domain.Enums.ReservationConfirmationStatus.RejectedByMedicalRecord;
                        reservationConf.RejectionAndCancellationUserId = operatorId;
                        reservationConf.RejectionAndCancellationAdditionalDescription = request.AdditionalDescription;
                        reservationConf.ReservationRejectionAndCancellationReasonId = request.ReservationRejectionReasonId;

                        await _reservationConfirmation.UpdateAsync(reservationConf);
                        return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
                    }
                }
                else
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست باید توسط مدارک پزشکی تعیین وضعیت شود" };
                }
            }
            else if (reservationConf.ReservationConfirmationType.Name == "Extera-Reservation") // نوبت های مازاد
            {
                if (user.Role.RoleName == "MedicalRecord")
                {
                    if (!reservationConf.IsConfirmedBySupervisor)
                    {
                        return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست اول باید توسط سوپروایز تعیین وضعیت شود" };
                    }
                    if (reservationConf.IsConfirmedByMedicalRecords)
                    {
                        return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست قبلا توسط  مدارک پزشکی تعیین وضعیت شده است" };
                    }
                    else
                    {
                        reservationConf.StatusId = (int)Domain.Enums.ReservationConfirmationStatus.RejectedByMedicalRecord;
                        reservationConf.RejectionAndCancellationAdditionalDescription = request.AdditionalDescription;
                        reservationConf.RejectionAndCancellationUserId = operatorId;
                        reservationConf.ReservationRejectionAndCancellationReasonId = request.ReservationRejectionReasonId;
                        await _reservationConfirmation.UpdateAsync(reservationConf);
                        return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
                    }
                }
                else if (user.Role.RoleName == "Supervisor")
                {
                    if (reservationConf.IsConfirmedBySupervisor)
                    {
                        return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست قبلا توسط  سوپروایزر تعیین وضعیت شده است" };
                    }
                    else
                    {
                        reservationConf.StatusId = (int)Domain.Enums.ReservationConfirmationStatus.RejectedBySupervisor;
                        reservationConf.RejectionAndCancellationUserId = operatorId;
                        reservationConf.RejectionAndCancellationAdditionalDescription = request.AdditionalDescription;
                        reservationConf.ReservationRejectionAndCancellationReasonId = request.ReservationRejectionReasonId;
                        await _reservationConfirmation.UpdateAsync(reservationConf);
                        return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
                    }
                }
            }
            return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.InternalServerError, Status = "Failed" };
        }

        public async Task<ResponseDto<bool>> UpdateReservationByReservationId(Guid reservationId, UpdateReservationDto request, Guid operatorId)
        {
            var reservation = await _reservationRepository.GetReservationById(reservationId);
            if (reservation == null)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Not Found" };
            }
            var mappedTiming = _mapper.Map(request, reservation);
            mappedTiming.ModifiedBy = operatorId;
            mappedTiming.IsModified = true;
            mappedTiming.UsageTime = request.RequestedTime;
            await _reservationRepository.UpdateAsync(mappedTiming);
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
        }

        public async Task<ResponseDto<IEnumerable<GetTimingDto>>> GetDoctorDayOffList(PaginationDto request, string noNezam, DateOnly startDate, DateOnly endDate)
        {
            var timingList = await _timingRepository.GetDoctorDayOffList(request, noNezam, startDate, endDate);

            return new ResponseDto<IEnumerable<GetTimingDto>>
            {
                IsSuccessFull = true,
                Data = timingList.List,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = timingList.TotalCount
            };




        }

        public async Task<ResponseDto<bool>> SubmitDoctorDayOff(SubmitDoctorDayOffDto request, Guid operatorId)
        {
            List<Timing> timingsList = new List<Timing>();
            foreach (var item in request.TimingId)
            {
                // work on Timing 
                var timingItem = await _timingRepository.GetTimingById(item);
                if (timingItem == null)
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Not Found" };
                }
                timingItem.IsActive = false;
                timingItem.IsModified = true;
                timingItem.ModifiedDate = DateTime.Now;
                timingItem.ModifiedBy = operatorId;
                await _timingRepository.UpdateAsync(timingItem);
                var timing = new Timing
                {
                    IsExtraTiming = true,
                    AssignedDoctorNoNezam = null,
                    AssignedRoomCode = timingItem.AssignedRoomCode,
                    ScheduledDate = timingItem.ScheduledDate,
                    ScheduledStartTime = timingItem.ScheduledStartTime,
                    ScheduledEndTime = timingItem.ScheduledEndTime,
                    ScheduledDuration = timingItem.ScheduledDuration,
                    CreatedDate_Shamsi = UtilityManager.GregorianDateTimeToPersianDate(DateTime.Now),
                    ScheduledDate_Shamsi = UtilityManager.GregorianDateTimeToPersianDateOnly(timingItem.ScheduledDate),
                    PreviousOwner = timingItem.AssignedDoctorNoNezam,
                };
                timingsList.Add(timing);

                // work on Reservation
                var getUser = await _userRepository.GetUserWithRolesByUserId(operatorId);
                var reservations = await _reservationRepository.GetReservationsByTimingId(item);
                foreach (var reservationItem in reservations)
                {

                    await _sender.SendPatientCanecllationSmsAsync(reservationItem.PatientPhoneNumber,reservationItem.PatientName,reservationItem.RequestedDate.ToString());

                    reservationItem.IsActive = false;
                    timingItem.ModifiedBy = operatorId;
                    reservationItem.ReservationConfirmation.StatusId = getUser.Role.RoleName == "Supervisor" ? (int)ReservationConfirmationStatus.CancelledBySupervisor : (int)ReservationConfirmationStatus.CancelledByMedicalRecord;
                    reservationItem.ReservationConfirmation.RejectionAndCancellationAdditionalDescription = "مرخصی پزشک";
                    await _reservationRepository.UpdateAsync(reservationItem);
                }
            }

            if (timingsList != null && timingsList.Count > 0)
            {
                await _timingRepository.AddRangeAsync(timingsList);
            }
            return new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Successful" };
        }

        public async Task<ResponseDto<IEnumerable<InsuranceDto>>> GetInsuranceList(string searchKey)
        {
            var insuranceList = await _doctorRepository.GetInsuranceList(searchKey);
            var insuranceListMapped = _mapper.Map<List<InsuranceDto>>(insuranceList);

            return new ResponseDto<IEnumerable<InsuranceDto>>
            {
                IsSuccessFull = true,
                Data = insuranceListMapped,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
            };
        }

        public async Task<ResponseDto<IEnumerable<SurgeryNameDto>>> GetSurgeryNamesList(string searchKey)
        {
            var surgeryNames = await _doctorRepository.GetSurgeryNamesList(searchKey);
            var surgeryNamesMapped = _mapper.Map<List<SurgeryNameDto>>(surgeryNames);

            return new ResponseDto<IEnumerable<SurgeryNameDto>>
            {
                IsSuccessFull = true,
                Data = surgeryNamesMapped,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
            };
        }
    }
}
