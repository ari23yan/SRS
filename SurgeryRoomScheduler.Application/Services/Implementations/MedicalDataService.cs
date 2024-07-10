using AutoMapper;
using Azure.Core;
using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Domain.Dtos;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Entities.Account;
using SurgeryRoomScheduler.Domain.Entities.General;
using SurgeryRoomScheduler.Domain.Interfaces;
using SurgeryRoomScheduler.Domain.Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Services.Implementations
{
    public class MedicalDataService : IMedicalDataService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public MedicalDataService(IDoctorRepository doctorRepository, IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<bool> DeleteDoctors()
        {
          return await _doctorRepository.DeleteDoctors();
        }
        public async Task<bool> DeleteDoctorsAssignedRooms()
        {
            return await _doctorRepository.DeleteDoctorsAssignedRooms();
        }
        public async Task<bool> DeleteInsurances()
        {
            return await _doctorRepository.DeleteInsurances();
        }
        public async Task<bool> DeleteRooms()
        {
            return await _doctorRepository.DeleteRooms();
        }
        public async Task<bool> DeleteSurgeryNames()
        {
            return await _doctorRepository.DeleteInsurances();
        }
        public async Task<ResponseDto<IEnumerable<DoctorListDto>>> GetDoctorList(long roomCode,string searchKey)
        {
            var doctorsList = await _doctorRepository.GetDoctorsList(roomCode,searchKey);
            if (doctorsList == null)
            {
                return new ResponseDto<IEnumerable<DoctorListDto>> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Not Found" };
            }
            var mappedDoctor = _mapper.Map<IEnumerable<Doctor>, IEnumerable<DoctorListDto>>(doctorsList);
            var DoctorsCount = await _doctorRepository.GetCountAsync(x => x.IsActive.Value);
            return new ResponseDto<IEnumerable<DoctorListDto>> { IsSuccessFull = true,Data = mappedDoctor, Message = ErrorsMessages.Success, Status = "Successful", TotalCount = string.IsNullOrEmpty(searchKey) == true ? DoctorsCount : doctorsList.Count() };
        }

        public async Task<ResponseDto<IEnumerable<RoomsListDto>>> GetDoctorRooms(string noNezam)
        {
            var roomList = await _doctorRepository.GetDoctorRooms(noNezam);
            if (roomList == null)
            {
                return new ResponseDto<IEnumerable<RoomsListDto>> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Not Found" };
            }
            var mappedRoom = _mapper.Map<IEnumerable<Room>, IEnumerable<RoomsListDto>>(roomList);
            return new ResponseDto<IEnumerable<RoomsListDto>> { IsSuccessFull = true, Data = mappedRoom, Message = ErrorsMessages.Success, Status = "Successful",TotalCount = roomList.Count() };
        }

        public async Task<ResponseDto<IEnumerable<RoomsListDto>>> GetRoomsList(string searchKey)
        {
            var roomList = await _doctorRepository.GetRoomsList(searchKey);
            if (roomList == null)
            {
                return new ResponseDto<IEnumerable<RoomsListDto>> { IsSuccessFull = false, Message = ErrorsMessages.NotFound, Status = "Not Found" };
            }
            var mappedRoom = _mapper.Map<IEnumerable<Room>, IEnumerable<RoomsListDto>>(roomList);
            var roomscount = await _doctorRepository.GetCountAsync(x => x.IsActive.Value);
            return new ResponseDto<IEnumerable<RoomsListDto>> { IsSuccessFull = true, Data = mappedRoom, Message = ErrorsMessages.Success, Status = "Successful", TotalCount = string.IsNullOrEmpty(searchKey) == true ? roomscount : roomList.Count() };
        }
    }
}
