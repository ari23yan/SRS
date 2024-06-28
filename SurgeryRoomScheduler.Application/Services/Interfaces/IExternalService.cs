using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Services.Interfaces
{
    public interface IExternalService
    {
        Task<ResponseDto<string>> Authenticate();
        Task<ResponseDto<string>> GetDoctorsList();
        Task<ResponseDto<string>> GetRoomsList();
        Task<ResponseDto<string>> GetInsuranceList();
        Task<ResponseDto<string>> GetSurgeryNamesList();
    }
}
