using SurgeryRoomScheduler.Domain.Dtos.Common.AccessLog;
using SurgeryRoomScheduler.Domain.Entities.Common;
using SurgeryRoomScheduler.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Services.Interfaces
{
    public interface ILogService
    {
        void InsertLog(string ip, string controllerName, string actionName, string userAgent, Exception ex);
        Task<bool> InsertJobLog(JobLog log);
        Task<bool> UpdateJobLog(JobLog log);
        Task<bool> InserAccessLog(AccessLogType type, InsertUserAccessLogDto request);
    }
}
