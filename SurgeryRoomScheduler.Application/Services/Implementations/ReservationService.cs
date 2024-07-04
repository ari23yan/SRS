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
using SurgeryRoomScheduler.Domain.Enums;
using Azure.Core;
using Microsoft.OpenApi.Models;

namespace SurgeryRoomScheduler.Application.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ITimingRepository _timingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IRepository<ReservationConfirmation> _reservationConfirmation;
        private readonly IRepository<ReservationRejection> _reservationRejection;
        private readonly IMapper _mapper;
        public ReservationService(IUserRepository userRepository, ISender sender, ITimingRepository timingRepository,
            IReservationRepository reservationRepository, IMapper mapper, IDoctorRepository doctorRepository,
             IRepository<ReservationRejection> reservationRejection,
            IRepository<ReservationConfirmation> reservationConfirmation)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _reservationRepository = reservationRepository;
            _timingRepository = timingRepository;
            _doctorRepository = doctorRepository;
            _reservationConfirmation = reservationConfirmation;
            _reservationRejection = reservationRejection;
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
            await _reservationConfirmation.UpdateAsync(reservationConf);
            reservation.IsCanceled = true;
            reservation.CancelationDescription = request.CancellationDescription;
            reservation.ReservationConfirmation.ReservationRejection.Id = request.ReservationRejectionReasonId;
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
            if (reservationConf.StatusId != (int)Domain.Enums.ReservationConfirmationStatus.Pending)
            {
                return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "این درخواست قبلا تعیین وضعیت شده است" };
            }

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
                request.DoctorNoNezam = timing.AssignedDoctorNoNezam;
                request.RoomCode = timing.AssignedRoomCode;
                if (timing.AssignedDoctorNoNezam != request.DoctorNoNezam)
                {
                    return new ResponseDto<bool> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "زمانبندی مورد نظر برای دکتر دیگری در نظر گرفته شده است" };
                }
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

        public async Task<ResponseDto<IEnumerable<TimingDto>>> GetExteraTimingsList(PaginationDto request, Guid? doctorId, long roomCode)
        {
            var doctor = await _userRepository.GetUserByUserId(doctorId.Value);
            if (doctor == null)
            {
                return new ResponseDto<IEnumerable<TimingDto>> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Failed" };
            }
            if (roomCode == 0)
            {
                var doctorsRooms = await _doctorRepository.GetDoctorRooms(doctor.NoNezam);
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
            var reservsCount = await GetExteraReservedCount(roomCode);
            return new ResponseDto<IEnumerable<TimingDto>>
            {
                IsSuccessFull = true,
                Data = filtredTiming,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = string.IsNullOrEmpty(request.Searchkey) == true ? reservsCount : filtredTiming.Count()
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

        public async Task<ResponseDto<IEnumerable<ReservationDto>>> GetPaginatedReservervationsList(PaginationDto request, Guid operatorId, ReservationStatus status)
        {
            var user = await _userRepository.GetUserWithRolesByUserId(operatorId);
            if (user == null)
            {
                return new ResponseDto<IEnumerable<ReservationDto>> { IsSuccessFull = false, Message = ErrorsMessages.UserNotfound, Status = "Failed" };
            }
            var reservs = await _reservationRepository.GetPaginatedReservervationsList(request, user.Role.RoleName, status);
            var reservsCount = await GetReservedConfirmationCountByType(user.Role.RoleName, status);

            return new ResponseDto<IEnumerable<ReservationDto>>
            {
                IsSuccessFull = true,
                Data = reservs,
                Message = ErrorsMessages.Success,
                Status = "SuccessFul",
                TotalCount = string.IsNullOrEmpty(request.Searchkey) == true ? reservsCount : reservs.Count()
            };
        }

        public async Task<ResponseDto<IEnumerable<ReservationRejectionAndCancellationReason>>> GetRejectionsReasons(Guid operatorId)
        {
            var user = await _userRepository.GetUserWithRolesByUserId(operatorId);
            if (user == null)
            {
                return new ResponseDto<IEnumerable<ReservationRejectionAndCancellationReason>> { IsSuccessFull = false, Message = ErrorsMessages.UserNotfound, Status = "Failed" };
            }
            IEnumerable<ReservationRejectionAndCancellationReason> records = new List<ReservationRejectionAndCancellationReason>();

            if (user.Role.RoleName == "Supervisor")
            {
                records = await GetReservationRejectionReasonByType(RejectionReasonType.Supervisor);
            }
            else if (user.Role.RoleName == "MedicalRecord")
            {
                records = await GetReservationRejectionReasonByType(RejectionReasonType.MedicalRecords);

            }
            else
            {
                records = await GetReservationRejectionReasonByType(RejectionReasonType.doctor);
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
                    Timings = filtredTiming.Where(u => u.ScheduledDate == DateOnly.FromDateTime(item.MiladiDate.Date)).ToList(),
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
            return await _timingRepository.GetCountAsync(x => x.IsActive && !x.IsDeleted && x.IsExtraTiming && x.AssignedRoomCode == roomCode);
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
                        var reservationRejection = new ReservationRejection
                        {
                            ReservationRejectionReasonId = request.ReservationRejectionReasonId,
                            ReservationId = request.ReservationId,
                            AdditionalDescription = request.AdditionalDescription,
                        };
                        await _reservationRejection.AddAsync(reservationRejection);

                        reservationConf.StatusId = (int)Domain.Enums.ReservationConfirmationStatus.RejectedByMedicalRecord;
                        reservationConf.ReservationRejectionId = reservationRejection.Id;
                        reservationConf.RejectionUserId = operatorId;
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
                        var reservationRejection = new ReservationRejection
                        {
                            ReservationRejectionReasonId = request.ReservationRejectionReasonId,
                            ReservationId = request.ReservationId,
                            AdditionalDescription = request.AdditionalDescription,
                        };
                        await _reservationRejection.AddAsync(reservationRejection);

                        reservationConf.StatusId = (int)Domain.Enums.ReservationConfirmationStatus.RejectedByMedicalRecord;
                        reservationConf.ReservationRejectionId = reservationRejection.Id;
                        reservationConf.RejectionUserId = operatorId;
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
                        var reservationRejection = new ReservationRejection
                        {
                            ReservationRejectionReasonId = request.ReservationRejectionReasonId,
                            ReservationId = request.ReservationId,
                            AdditionalDescription = request.AdditionalDescription,
                        };
                        await _reservationRejection.AddAsync(reservationRejection);

                        reservationConf.StatusId = (int)Domain.Enums.ReservationConfirmationStatus.RejectedBySupervisor;
                        reservationConf.ReservationRejectionId = reservationRejection.Id;
                        reservationConf.RejectionUserId = operatorId;
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
    }
}
