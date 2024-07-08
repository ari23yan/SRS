using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Application.Utilities;
using SurgeryRoomScheduler.Domain.Dtos.Common.AccessLog;
using SurgeryRoomScheduler.Domain.Entities.Common;
using SurgeryRoomScheduler.Domain.Enums;
using SurgeryRoomScheduler.Domain.Interfaces;
using SurgeryRoomScheduler.Domain.Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Services.Implementations
{
    public class LogService : ILogService
    {
        private protected IRepository<ApplicationLog> _logRepository;
        private protected IRepository<JobLog> _jobsLog;
        private protected IUserRepository _userRepository;
        private readonly IRepository<UsersAccessLog> _usersAccessLogRepository;

        public LogService(IRepository<ApplicationLog> logRepository, IRepository<UsersAccessLog> usersAccessLogRepository
            , IUserRepository userRepository, IRepository<JobLog> jobsLog)
        {
            _logRepository = logRepository;
            _jobsLog = jobsLog;
            _userRepository = userRepository;
            _usersAccessLogRepository = usersAccessLogRepository;
        }

        public async Task<bool> InsertJobLog(JobLog log)
        {
            await _jobsLog.AddAsync(log);
            return true;
        }
        public async Task<bool> InserAccessLog(AccessLogType type, InsertUserAccessLogDto request)
        {
            var operatorUser = await _userRepository.GetUserByUserId(request.OperatorId);
            var user = _userRepository.GetUserByUserId(request.UserId);
            UsersAccessLog usersAccess = new UsersAccessLog();
            usersAccess.OperatorId = request.OperatorId;
            usersAccess.UserId = request.UserId;
            var name = operatorUser.FirstName + " " + operatorUser.LastName;
            string persianDate = UtilityManager.ConvertGregorianDateTimeToPersianDate(DateTime.Now);
            if (type == AccessLogType.RoleChange)
            {
                usersAccess.Type = AccessLogType.RoleChange;
                usersAccess.Action = "تغییر نقش کاربر توسط " + name + " در تاریخ " + persianDate + " صورت گرفته است";
            }
            else if (type == AccessLogType.PermissionChange)
            {
                usersAccess.Type = AccessLogType.PermissionChange;
                usersAccess.Action = "تغییر سطح دسترسی کاربر توسط " + name + " در تاریخ " + persianDate + " صورت گرفته است";
            }
            else if (type == AccessLogType.MenuChange)
            {
                usersAccess.Type = AccessLogType.MenuChange;
                usersAccess.Action = "تغییر منوی  کاربر توسط " + name + " در تاریخ " + persianDate + " صورت گرفته است";
            }
            await _usersAccessLogRepository.AddAsync(usersAccess);
            return true;
        }

        public async void InsertLog(string ip, string controllerName, string actionName, string userAgent, Exception ex)
        {
            ApplicationLog log = new ApplicationLog
            {
                ActionName = actionName,
                ControllerName = controllerName,
                Exception = ex == null ? null : ex.ToString(),
                IpAddress = ip,
                Message = ex.Message == null ? null : ex.Message,
                Source = ex.Source == null ? null : ex.Source,
                InnerException = ex.InnerException == null ? null : ex.InnerException.ToString(),
                Timestamp = DateTime.Now,
                UserAgent = userAgent,
            };
            await _logRepository.AddAsync(log);
        }

        public async Task<bool> UpdateJobLog(JobLog log)
        {
            await _jobsLog.UpdateAsync(log);
            return true;
        }
    }
}
