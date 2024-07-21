using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Plugins;
using SurgeryRoomScheduler.Application.Jobs.Interfaces;
using SurgeryRoomScheduler.Application.Security;
using SurgeryRoomScheduler.Application.Senders;
using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Application.Utilities;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos.Sender;
using SurgeryRoomScheduler.Domain.Dtos.User;
using SurgeryRoomScheduler.Domain.Enums;
using SurgeryRoomScheduler.Presentation.Controllers.UserSide.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ISender = SurgeryRoomScheduler.Application.Senders.ISender;


namespace SurgeryRoomScheduler.Presentation.Controllers.UserSide
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogService _logService;
        private readonly ISender _sender;
        private readonly IJobs _jobs;


        public AccountController(IUserService userService, ILogService logService,
        IHttpContextAccessor httpContextAccessor, ISender sender, IJobs jobs,
        IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logService = logService;
            _sender = sender;
            _jobs = jobs;
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginUserDto request)
        {
            try
            {
                var result = await _userService.LoginUser(request);
                switch (result.Result)
                {
                    case UserAuthResponse.WrongPassword:
                        return Unauthorized(new ResponseDto<bool> { IsSuccessFull = false, Data = false, Message = ErrorsMessages.UserORPasswrodIsWrong, Status = "UserORPasswrodIsWrong" });
                    case UserAuthResponse.TooManyTries:
                        return Unauthorized(new ResponseDto<bool> { IsSuccessFull = false, Data = false, Message = ErrorsMessages.Faild, Status = "تعداد دفعات ورود به سامانه از حد مجاز گذشته است" });
                    case UserAuthResponse.NotAvtive:
                        return Unauthorized(new ResponseDto<string> { IsSuccessFull = false, Data = result.user.PhoneNumber, Message = ErrorsMessages.NotActive, Status = "UserAccountIsNotActive" }); // redirect to ActiveAccount
                    case UserAuthResponse.NotFound:
                        return Unauthorized(new ResponseDto<bool> { IsSuccessFull = false, Data = false, Message = ErrorsMessages.UserNotfound, Status = "NotFound" });
                    case UserAuthResponse.Success:
                        var userOtp = UtilityManager.OtpGenrator();
                        if (result.user.RoleId.ToString() != "f2a3bbee-ee97-4cb6-ab48-fbce464c0c56") //Developer
                        {
                            var loginSms = await _sender.LoginUserSendSmsAsync(result.user.PhoneNumber, userOtp);
                            if (!loginSms)
                            {
                                return Ok(new ResponseDto<string> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "اشکال در ارسال پیامک" }); // redirect to ActiveAccount
                            }
                            await _userService.InsertOtp(userOtp, result.user.Id);
                        }
                        else
                            await _userService.InsertOtp("123456", result.user.Id);

                        return Ok(new ResponseDto<string> { IsSuccessFull = true, Data = result.user.PhoneNumber, Message = ErrorsMessages.SuccessLogin, Status = "پیامک ورود برای شماره تلفن شما ارسال شد" });
                    default:
                        return BadRequest(new ResponseDto<bool> { IsSuccessFull = false, Data = false, Message = "Unexpected result", Status = "UnexpectedResult" });
                }
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
        public async Task<IActionResult> ConfirmOtp(ConfirmOtpDto request)
        {
            try
            {
                var result = await _userService.ConfirmOtp(request);
                if (result.IsSuccessFull.Value)
                {
                    var claims = new List<Claim>
                   {
                    new Claim(ClaimTypes.NameIdentifier,result.Data.Id.ToString()),
                    new Claim(ClaimTypes.Role,result.Data.RoleMenus.Id.ToString())
                   };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:IssuerSigningKey"]));
                    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        issuer: _configuration["Authentication:Issuer"],
                        audience: _configuration["Authentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddDays(2),
                        signingCredentials: credentials
                    );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                    result.Data.Token = tokenString;
                    return Ok(result);
                }
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
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
        {
            try
            {
                var result = await _userService.ForgotPassword(request);
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
        public async Task<IActionResult> SubmitPassword(SubmitPasswordDto request)
        {
            try
            {
                var result = await _userService.SubmitPassword(request);
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
        public async Task<IActionResult> SignOut()
        {
            try
            {
                HttpContext.SignOutAsync();
                return Ok(new ResponseDto<bool> { IsSuccessFull = true, Message = ErrorsMessages.Success, Status = "Success" });
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

    }
}
