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

namespace SurgeryRoomScheduler.Application.Jobs.Implementations
{
    public class Jobs : IJobs
    {
        private readonly IExternalService _externalService;
        private readonly IUserService _userService;
        private readonly IRepository<Doctor> _docRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IMedicalDataService _medicalDataService;
        private readonly ILogService _logService;
        private readonly IMapper _mapper;
        public Jobs(IExternalService externalService, ILogService logService,
            IMapper mapper,IUserService userService, IRepository<User> userrepository, IRepository<Doctor> docRepository
            , IMedicalDataService medicalDataService)
        {
            _externalService = externalService;
            _logService = logService;
            _mapper = mapper;
            _medicalDataService = medicalDataService;
            _userService = userService;
            _docRepository = docRepository;
            _userRepository = userrepository;
        }
        public async Task<bool> GetDoctorsListJob()
        {
            var log = new JobLog { JobName = "Doctors List", StartTime = DateTime.Now };
            try
            {
                var doctorsListService = await _externalService.GetDoctorsList();
                if (doctorsListService != null && doctorsListService.IsSuccessFull.Value)
                {
                    try
                    {
                        var doctors = JsonConvert.DeserializeObject<List<DoctorDto>>(doctorsListService.Data);
                        var uniqeDoctors = doctors.DistinctBy(x => x.NoNezam).ToList();

                        var mappedDoctors = _mapper.Map<List<DoctorDto>,List <Doctor>> (uniqeDoctors);

                        var deleteDoctorsOldDataFromDoctorTable = await _medicalDataService.DeleteDoctors();
                        await _docRepository.AddRangeAsync(mappedDoctors);

                        var mappedUsersDoctors = _mapper.Map<List<DoctorDto>, List<User>>(uniqeDoctors);

                        var deleteDoctorOldDataFromUserTable = await _userService.DeleteDoctors();
                         await _userRepository.AddRangeAsync(mappedUsersDoctors);
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = true;
                        log.Description = "Total Doctor Count = " + doctors.Count();
                        await _logService.InsertJobLog(log);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        log.EndTime = DateTime.Now;
                        log.IsSuccessful = false;
                        log.ErrorDetails = ex.ToString();
                        log.Description = "Exception On Deserialize Doctor Object";
                        await _logService.InsertJobLog(log);
                        return false;
                    }

                }
                return false;

            }
            catch (Exception ex)
            {

                throw;
            }
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
