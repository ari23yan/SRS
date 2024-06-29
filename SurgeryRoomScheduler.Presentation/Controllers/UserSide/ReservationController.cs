using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SurgeryRoomScheduler.Application.Senders;
using SurgeryRoomScheduler.Application.Services.Implementations;
using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Application.Utilities;
using SurgeryRoomScheduler.Domain.Dtos.Common.Pagination;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Reservation;
using SurgeryRoomScheduler.Domain.Dtos.Timing;
using SurgeryRoomScheduler.Domain.Enums;
using SurgeryRoomScheduler.Presentation.Controllers.UserSide.Common;

namespace SurgeryRoomScheduler.Presentation.Controllers.UserSide
{
    public class ReservationController : BaseController
    {

        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogService _logService;
        private readonly IReservationService _reservationService;

        public ReservationController(IUserService userService, ILogService logService,
        IHttpContextAccessor httpContextAccessor, IReservationService reservationService,
        IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logService = logService;
            _reservationService = reservationService;
        }


        [HttpGet]
        [PermissionChecker(Permission = PermissionType.GetDoctorReservedList)]
        public async Task<IActionResult> GetList([FromQuery] PaginationDto request)
        {
            try
            {
                var currentUser = UtilityManager.GetCurrentUser(_httpContextAccessor);

                var result = await _reservationService.GetPaginatedReservedList(request, currentUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                #region Inserting Log 
                if (_configuration.GetValue<bool>("ApplicationLogIsActive"))
                {
                    var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"];
                    var userIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                    var routeData = ControllerContext.RouteData;
                    var controllerName = routeData.Values["controller"]?.ToString();
                    var actionName = routeData.Values["action"]?.ToString();
                    _logService.InsertLog(userIp, controllerName, actionName, userAgent, ex);
                }
                #endregion
                return Ok(new ResponseDto<Exception> { IsSuccessFull = false, Data = ex, Message = ErrorsMessages.InternalServerError, Status = "Internal Server Error" });
            }
        }


        [HttpGet]
        [PermissionChecker(Permission = PermissionType.GetReservationCalender)]
        public async Task<IActionResult> GetReservationCalender([FromQuery] GetListByMonthDto request)
        {
            try
            {
                var currentUser = UtilityManager.GetCurrentUser(_httpContextAccessor);
                request.UserId = currentUser;
                var result = await _reservationService.GetReservationCalender(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                #region Inserting Log 
                if (_configuration.GetValue<bool>("ApplicationLogIsActive"))
                {
                    var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"];
                    var userIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                    var routeData = ControllerContext.RouteData;
                    var controllerName = routeData.Values["controller"]?.ToString();
                    var actionName = routeData.Values["action"]?.ToString();
                    _logService.InsertLog(userIp, controllerName, actionName, userAgent, ex);
                }
                #endregion
                return Ok(new ResponseDto<Exception> { IsSuccessFull = false, Data = ex, Message = ErrorsMessages.InternalServerError, Status = "Internal Server Error" });
            }
        }


        [HttpPost]
        [PermissionChecker(Permission = PermissionType.AddReservation)]
        public async Task<IActionResult> Add(AddReservationDto request)
        {
            try
            {
                var currentUser = UtilityManager.GetCurrentUser(_httpContextAccessor);
                var result = await _reservationService.CreateReservation(request, currentUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                #region Inserting Log 
                if (_configuration.GetValue<bool>("ApplicationLogIsActive"))
                {
                    var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"];
                    var userIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                    var routeData = ControllerContext.RouteData;
                    var controllerName = routeData.Values["controller"]?.ToString();
                    var actionName = routeData.Values["action"]?.ToString();
                    _logService.InsertLog(userIp, controllerName, actionName, userAgent, ex);
                }
                #endregion
                return Ok(new ResponseDto<Exception> { IsSuccessFull = false, Data = ex, Message = ErrorsMessages.InternalServerError, Status = "Internal Server Error" });
            }
        }


        [HttpPut]
        //[PermissionChecker(Permission = PermissionType.Admin_UpdateTiming)]
        public async Task<IActionResult> Update(Guid reservationId, UpdateReservationDto request)
        {
            try
            {
                var currentUser = UtilityManager.GetCurrentUser(_httpContextAccessor);
                var result = await _reservationService.UpdateReservationByReservationId(reservationId, request, currentUser);
                return Ok(result);
            }
            catch (Exception ex)
            {
                #region Inserting Log 
                if (_configuration.GetValue<bool>("ApplicationLogIsActive"))
                {

                    var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"];
                    var userIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
                    var routeData = ControllerContext.RouteData;
                    var controllerName = routeData.Values["controller"]?.ToString();
                    var actionName = routeData.Values["action"]?.ToString();
                    _logService.InsertLog(userIp, controllerName, actionName, userAgent, ex);
                }
                #endregion
                return Ok(new ResponseDto<Exception> { IsSuccessFull = false, Data = ex, Message = ErrorsMessages.InternalServerError, Status = "Internal Server Error" });
            }
        }


        //public async Task<IActionResult> CancelReservation(Guid reservationId)
        //{
        //    try
        //    {
        //        var currentUser = UtilityManager.GetCurrentUser(_httpContextAccessor);
        //        var result = await _reservationService.UpdateReservationByReservationId(reservationId, request, currentUser);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        #region Inserting Log 
        //        if (_configuration.GetValue<bool>("ApplicationLogIsActive"))
        //        {

        //            var userAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"];
        //            var userIp = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        //            var routeData = ControllerContext.RouteData;
        //            var controllerName = routeData.Values["controller"]?.ToString();
        //            var actionName = routeData.Values["action"]?.ToString();
        //            _logService.InsertLog(userIp, controllerName, actionName, userAgent, ex);
        //        }
        //        #endregion
        //        return Ok(new ResponseDto<Exception> { IsSuccessFull = false, Data = ex, Message = ErrorsMessages.InternalServerError, Status = "Internal Server Error" });
        //    }
        //}



    }
}
