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

namespace SurgeryRoomScheduler.Application.Jobs.Implementations
{
    public class Jobs : IJobs
    {
        private readonly IExternalService _externalService;
        private readonly IUserService _userService;
        private readonly IRepository<Doctor> _docRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Timing> _timingRepository;
        private readonly IMedicalDataService _medicalDataService;
        private readonly ITimingService _timingService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        public Jobs(IExternalService externalService, ILogService logService, IRepository<Timing> timingRepository,
            IMapper mapper, ITimingService timingService, IUserService userService, IRepository<User> userrepository, IRepository<Doctor> docRepository
            , IMedicalDataService medicalDataService)
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
        }

        public async Task<bool> ExteraTimingJob()
        {
            var log = new JobLog { JobName = "ExteraTiming", StartTime = DateTime.Now, IsSuccessful = false };
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
                    _timingRepository.UpdateAsync(timingItem);
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
            var log = new JobLog { JobName = "DoctorsList", StartTime = DateTime.Now, IsSuccessful = false };
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
                        log.Description = "Exception On Deserialize Doctor Object";
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

        public Task<bool> GetDoctorsRoomJob()
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetInsuranceListJob()
        {
            throw new NotImplementedException();
        }
        public Task<bool> GetRoomListJob()
        {
            throw new NotImplementedException();
        }
        public Task<bool> GetSurgeryNamesListJob()
        {
            throw new NotImplementedException();
        }
    }
}
