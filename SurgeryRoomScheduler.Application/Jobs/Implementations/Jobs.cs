using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SurgeryRoomScheduler.Application.Jobs.Interfaces;
using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos;
using SurgeryRoomScheduler.Domain.Dtos.Jobs;
using SurgeryRoomScheduler.Domain.Dtos.Role;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Entities.Common;
using SurgeryRoomScheduler.Domain.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SurgeryRoomScheduler.Domain.Interfaces;
using Azure.Core;
using SurgeryRoomScheduler.Application.Utilities;
using SurgeryRoomScheduler.Data.Repositories;
using SurgeryRoomScheduler.Domain.Dtos.Doctor;
using SurgeryRoomScheduler.Domain.Dtos.Insurance;
using SurgeryRoomScheduler.Domain.Dtos.SurgeryName;

namespace SurgeryRoomScheduler.Application.Jobs.Implementations
{
    public class Jobs : IJobs
    {
        private readonly IExternalService _externalService;
        private readonly IUserService _userService;
        private readonly IRepository<Doctor> _docRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Timing> _timingRepository;
        private readonly IRepository<Room> _roomRepository;
        private readonly IRepository<DoctorRoom> _docRoomRepository;
        private readonly IRepository<Insurance> _insuranceRepository;
        private readonly IRepository<SurgeryName> _surgeryNamerepository;
        private readonly IMedicalDataService _medicalDataService;
        private readonly ITimingService _timingService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        public Jobs(IExternalService externalService, ILogService logService, IRepository<Timing> timingRepository,
            IMapper mapper, ITimingService timingService, IUserService userService, IRepository<User> userrepository, IRepository<Doctor> docRepository
            , IMedicalDataService medicalDataService, IRepository<DoctorRoom> docRoomRepository, IRepository<Insurance> insuranceRepository,
            IRepository<Room> roomRepository, IRepository<SurgeryName> surgeryNamerepository
            )
        {
            _externalService = externalService;
            _logService = logService;
            _mapper = mapper;
            _medicalDataService = medicalDataService;
            _userService = userService;
            _docRepository = docRepository;
            _userRepository = userrepository;
            _timingService = timingService;
            _timingRepository = timingRepository;
            _docRoomRepository = docRoomRepository;
            _insuranceRepository = insuranceRepository;
            _insuranceRepository = insuranceRepository;
            _roomRepository = roomRepository;
            _surgeryNamerepository = surgeryNamerepository;
        }

        public async Task<bool> ExteraTimingJob()
        {
            var log = new JobLog { JobName = "ExteraTimingJob", StartTime = DateTime.Now, IsSuccessful = false };
            await _logService.InsertJobLog(log);
            try
            {
                var date = DateOnly.FromDateTime(DateTime.Now).AddDays(-3);
                var timings = await _timingService.GetExteraTimingListByDate(date); // Three Days Ago Timings

                List<Timing> timingsList = new List<Timing>();
                // Not Used Timings

                foreach (var timingItem in timings.UnreservedTimings)
                {
                    timingItem.IsActive = false;
                    timingItem.IsModified = true;
                    timingItem.ModifiedDate = DateTime.Now;
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
                }

                // Not Fully Used Timings

                foreach (var timingItem in timings.NotFullyReservedTimings)
                {
                    var oldTiming = await _timingService.GetTimingByTimingId(timingItem.TimingId);
                    oldTiming.IsActive = false;
                    oldTiming.IsModified = true;
                    oldTiming.ModifiedDate = DateTime.Now;
                    await _timingRepository.UpdateAsync(oldTiming);

                    var timing = new Timing
                    {
                        IsExtraTiming = true,
                        AssignedDoctorNoNezam = null,
                        AssignedRoomCode = timingItem.AssignedRoomCode,
                        ScheduledDate = timingItem.ScheduledDate,
                        ScheduledStartTime = timingItem.ScheduledEndTime,
                        ScheduledEndTime = timingItem.ScheduledEndTime.Add(timingItem.UsageTime),
                        ScheduledDuration = timingItem.UsageTime,
                        CreatedDate_Shamsi = UtilityManager.GregorianDateTimeToPersianDate(DateTime.Now),
                        ScheduledDate_Shamsi = UtilityManager.GregorianDateTimeToPersianDateOnly(timingItem.ScheduledDate),
                        PreviousOwner = timingItem.AssignedDoctorNoNezam,
                    };
                    timingsList.Add(timing);
                }
                if (timingsList != null && timingsList.Count > 0)
                {
                   await _timingRepository.AddRangeAsync(timingsList);
                }

                log.EndTime = DateTime.Now;
                log.IsSuccessful = true;
                log.Description = "Total Timing Count = " + timingsList.Count();
                await _logService.UpdateJobLog(log);
                return true;
            }
            catch (Exception ex)
            {
                log.EndTime = DateTime.Now;
                log.IsSuccessful = false;
                log.ErrorDetails = ex.ToString();
                log.Description = "Exception";
                await _logService.UpdateJobLog(log);
                return false;
            }
        }
        public async Task<bool> GetDoctorsListJob()
        {
            var log = new JobLog { JobName = "DoctorsListJob", StartTime = DateTime.Now, IsSuccessful = false };
            await _logService.InsertJobLog(log);
            try
            {
                var doctorsListService = await _externalService.GetDoctorsList();
                if (doctorsListService != null && doctorsListService.IsSuccessFull.Value)
                {
                    try
                    {
                        var doctors = JsonConvert.DeserializeObject<List<DoctorDto>>(doctorsListService.Data);
                        var uniqeDoctors = doctors.DistinctBy(x => x.NoNezam).ToList();

                        var mappedDoctors = _mapper.Map<List<DoctorDto>, List<Doctor>>(uniqeDoctors);

                        var deleteDoctorsOldDataFromDoctorTable = await _medicalDataService.DeleteDoctors();
                        await _docRepository.AddRangeAsync(mappedDoctors);

                        var mappedUsersDoctors = _mapper.Map<List<DoctorDto>, List<User>>(uniqeDoctors);

                        var deleteDoctorOldDataFromUserTable = await _userService.DeleteDoctors();
                        await _userRepository.AddRangeAsync(mappedUsersDoctors);
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = true;
                        log.Description = "Total Doctor Count = " + doctors.Count();
                        await _logService.UpdateJobLog(log);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = false;
                        log.ErrorDetails = ex.ToString();
                        log.Description = "Exception";
                        await _logService.UpdateJobLog(log);
                        return false;
                    }
                }
                log.EndTime = DateTime.Now;
                log.IsSuccessful = false;
                log.ErrorDetails = "Authentication";
                log.Description = "Auth Is Not SuccessFull";
                await _logService.UpdateJobLog(log);
                return false;
            }
            catch (Exception ex)
            {
                log.EndTime = DateTime.Now;
                log.IsSuccessful = false;
                log.ErrorDetails = ex.ToString();
                log.Description = "Exception";
                await _logService.UpdateJobLog(log);
                return false;
            }
        }
        public async Task<bool> GetDoctorsAssignedRooms()
        {
            var log = new JobLog { JobName = "DoctorsAssignedRoomsJob", StartTime = DateTime.Now, IsSuccessful = false };
            await _logService.InsertJobLog(log);
            try
            {
                var doctorsAssignedRoomsService = await _externalService.GetDoctorsAssignedRooms();
                if (doctorsAssignedRoomsService != null && doctorsAssignedRoomsService.IsSuccessFull.Value)
                {
                    try
                    {
                        var rooms = JsonConvert.DeserializeObject<List<DoctorRoomDto>>(doctorsAssignedRoomsService.Data);
                        var mappedDoctorRoomss = _mapper.Map<List<DoctorRoomDto>, List<DoctorRoom>>(rooms);
                        await _medicalDataService.DeleteDoctorsAssignedRooms();
                        await _docRoomRepository.AddRangeAsync(mappedDoctorRoomss);
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = true;
                        log.Description = "Total Doctor Assigned Rooms Count = " + mappedDoctorRoomss.Count();
                        await _logService.UpdateJobLog(log);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = false;
                        log.ErrorDetails = ex.ToString();
                        log.Description = "Exception";
                        await _logService.UpdateJobLog(log);
                        return false;
                    }
                }
                log.EndTime = DateTime.Now;
                log.IsSuccessful = false;
                log.ErrorDetails = "Authentication";
                log.Description = doctorsAssignedRoomsService.Data;
                await _logService.UpdateJobLog(log);
                return false;
            }
            catch (Exception ex)
            {
                log.EndTime = DateTime.Now;
                log.IsSuccessful = false;
                log.ErrorDetails = ex.ToString();
                log.Description = "Exception";
                await _logService.UpdateJobLog(log);
                return false;
            }
        }
        public async Task<bool> GetInsuranceListJob()
        {
            var log = new JobLog { JobName = "InsuranceListJob", StartTime = DateTime.Now, IsSuccessful = false };
            await _logService.InsertJobLog(log);
            try
            {
                var insuranceListService = await _externalService.GetInsuranceList();
                if (insuranceListService != null && insuranceListService.IsSuccessFull.Value)
                {
                    try
                    {
                        var Insurances = JsonConvert.DeserializeObject<List<InsuranceDto>>(insuranceListService.Data);
                        var mappedInsurances = _mapper.Map<List<InsuranceDto>, List<Insurance>>(Insurances);
                        await _medicalDataService.DeleteInsurances();
                        await _insuranceRepository.AddRangeAsync(mappedInsurances);
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = true;
                        log.Description = "Total Insurances Count = " + mappedInsurances.Count();
                        await _logService.UpdateJobLog(log);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = false;
                        log.ErrorDetails = ex.ToString();
                        log.Description = "Exception";
                        await _logService.UpdateJobLog(log);
                        return false;
                    }
                }
                log.EndTime = DateTime.Now;
                log.IsSuccessful = false;
                log.ErrorDetails = "Authentication";
                log.Description = insuranceListService.Data;
                await _logService.UpdateJobLog(log);
                return false;
            }
            catch (Exception ex)
            {
                log.EndTime = DateTime.Now;
                log.IsSuccessful = false;
                log.ErrorDetails = ex.ToString();
                log.Description = "Exception";
                await _logService.UpdateJobLog(log);
                return false;
            }
        }
        public async Task<bool> GetRoomListJob()
        {
            var log = new JobLog { JobName = "RoomListJob", StartTime = DateTime.Now, IsSuccessful = false };
            await _logService.InsertJobLog(log);
            try
            {
                var roomListService = await _externalService.GetRoomsList();
                if (roomListService != null && roomListService.IsSuccessFull.Value)
                {
                    try
                    {
                        var rooms = JsonConvert.DeserializeObject<List<RoomDto>>(roomListService.Data);
                        var mappedRooms = _mapper.Map<List<RoomDto>, List<Room>>(rooms);
                        await _medicalDataService.DeleteRooms();
                        await _roomRepository.AddRangeAsync(mappedRooms);
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = true;
                        log.Description = "Total Rooms Count = " + mappedRooms.Count();
                        await _logService.UpdateJobLog(log);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = false;
                        log.ErrorDetails = ex.ToString();
                        log.Description = "Exception";
                        await _logService.UpdateJobLog(log);
                        return false;
                    }
                }
                log.EndTime = DateTime.Now;
                log.IsSuccessful = false;
                log.ErrorDetails = "Authentication";
                log.Description = roomListService.Data;
                await _logService.UpdateJobLog(log);
                return false;
            }
            catch (Exception ex)
            {
                log.EndTime = DateTime.Now;
                log.IsSuccessful = false;
                log.ErrorDetails = ex.ToString();
                log.Description = "Exception";
                await _logService.UpdateJobLog(log);
                return false;
            }
        }
        public async Task<bool> GetSurgeryNamesListJob()
        {
            var log = new JobLog { JobName = "SurgeryNamesListJob", StartTime = DateTime.Now, IsSuccessful = false };
            await _logService.InsertJobLog(log);
            try
            {
                var surgeryNamesListService = await _externalService.GetSurgeryNamesList();
                if (surgeryNamesListService != null && surgeryNamesListService.IsSuccessFull.Value)
                {
                    try
                    {
                        var surgeryNames = JsonConvert.DeserializeObject<List<SurgeryNameDto>>(surgeryNamesListService.Data);
                        var mappedSurgeryNames = _mapper.Map<List<SurgeryNameDto>, List<SurgeryName>>(surgeryNames);
                        await _medicalDataService.DeleteSurgeryNames();
                        await _surgeryNamerepository.AddRangeAsync(mappedSurgeryNames);
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = true;
                        log.Description = "Total SurgeryNames Count = " + mappedSurgeryNames.Count();
                        await _logService.UpdateJobLog(log);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = false;
                        log.ErrorDetails = ex.ToString();
                        log.Description = "Exception";
                        await _logService.UpdateJobLog(log);
                        return false;
                    }
                }
                log.EndTime = DateTime.Now;
                log.IsSuccessful = false;
                log.ErrorDetails = "Authentication";
                log.Description = surgeryNamesListService.Data;
                await _logService.UpdateJobLog(log);
                return false;
            }
            catch (Exception ex)
            {
                log.EndTime = DateTime.Now;
                log.IsSuccessful = false;
                log.ErrorDetails = ex.ToString();
                log.Description = "Exception";
                await _logService.UpdateJobLog(log);
                return false;
            }
        }
    }
}
