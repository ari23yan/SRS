using SurgeryRoomScheduler.Application.Security;
using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Domain.Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Services.Implementations
{
    public class PermissionService : IPermissionService
    {
        private readonly IUserRepository _userRepository;
        public PermissionService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<bool> CheckUserHavePermission(Guid roleId, Guid permissionId)
        {
            return await _userRepository.CheckUserHavePermission(roleId, permissionId);
        }
    }
}
