using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Jobs.Interfaces
{
    public interface IJobs
    {
        Task<bool> GetDoctorsListJob();
        Task<bool> GetRoomListJob();
        Task<bool> GetInsuranceListJob();
        Task<bool> GetSurgeryNamesListJob();
        Task<bool> GetDoctorsRoomJob();
        Task<bool> ExteraTimingJob();
    }
}
