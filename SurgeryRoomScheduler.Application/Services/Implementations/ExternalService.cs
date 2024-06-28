using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Services.Implementations
{
    public class ExternalService : IExternalService
    {
        private readonly IConfiguration _configuration;
        private readonly string? middlewareAddress;
        private readonly string? Username;
        private readonly string? Password;
        private readonly string? SecretId;

        public ExternalService(IConfiguration configuration)
        {
            _configuration = configuration;
            middlewareAddress = configuration.GetValue<string>("MiddlewareUrlAddress");
            Username = configuration.GetValue<string>("MiddlewareUsername");
            Password = configuration.GetValue<string>("MiddlewarePassword");
            SecretId = configuration.GetValue<string>("MiddlewareSecretId");
        }
        public async Task<ResponseDto<string>> Authenticate()
        {
            try
            {
                if(middlewareAddress.IsNullOrEmpty() || Username.IsNullOrEmpty() || Password.IsNullOrEmpty() || SecretId.IsNullOrEmpty())
                {
                    return new ResponseDto<string> { IsSuccessFull = false, Message = ErrorsMessages.Faild, Status = "کانفیگ های ارتباطی در اپ ستینگ یافت نشد." };
                }
                string requestBody = $"Username={Uri.EscapeDataString(Username)}&Password={ Uri.EscapeDataString(Password)}";
                byte[] requestBytes = Encoding.UTF8.GetBytes(requestBody);

                using (var httpClient = new HttpClient())
                {
                    var requestMessage = new HttpRequestMessage(HttpMethod.Post, middlewareAddress + "Api/Authenticate")
                    {
                        Content = new ByteArrayContent(requestBytes)
                    };
                    requestMessage.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(requestMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseBody = await response.Content.ReadAsStringAsync();
                        var responseDto = JsonConvert.DeserializeObject<ResponseDto<string>>(responseBody);
                        return responseDto;
                    }
                    else
                    {
                        return new ResponseDto<string> { IsSuccessFull = false,Data = response.StatusCode.ToString() + "  /  " + response.Content.ToString(), Message = ErrorsMessages.Faild, Status = "Api Response Status Code Is Not 200" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseDto<string> { IsSuccessFull = false,Data = ex.Message, Message = ErrorsMessages.InternalServerError, Status = "Exception" };
            }
            return new ResponseDto<string>();
        }
        public async Task<ResponseDto<string>> GetDoctorsList()
        {
            var authenticateResponse = await Authenticate();
            if (authenticateResponse.IsSuccessFull.Value == false)
            {
                return authenticateResponse;
            }
            using (var httpClient = new HttpClient())
            {
                // Set JWT token in request headers
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticateResponse.Data);
                try
                {
                    var url = $"{middlewareAddress}Api/GetDoctorsList?secretId={WebUtility.UrlEncode(SecretId)}&querySize=5000";
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        //var responseDto = JsonConvert.DeserializeObject<ResponseDto<string>>(responseBody);
                        return new ResponseDto<string> { IsSuccessFull = true, Data = responseBody, Message = ErrorsMessages.Success, Status = "SuccessFul" };
                    }
                    else
                    {
                        return new ResponseDto<string> { IsSuccessFull = false, Data = response.StatusCode.ToString() + "  /  " + response.Content.ToString(), Message = ErrorsMessages.Faild, Status = "Api Response Status Code Is Not 200" };
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseDto<string> { IsSuccessFull = false, Data = ex.Message, Message = ErrorsMessages.InternalServerError, Status = "Exception" };
                }
            }
        }
        public async Task<ResponseDto<string>> GetInsuranceList()
        {
            var authenticateResponse = await Authenticate();
            if (authenticateResponse.IsSuccessFull.Value == false)
            {
                return authenticateResponse;
            }
            using (var httpClient = new HttpClient())
            {
                // Set JWT token in request headers
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticateResponse.Data);
                try
                {
                    var url = $"{middlewareAddress}Api/GetInsuranceList?secretId={WebUtility.UrlEncode(SecretId)}";
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var responseDto = JsonConvert.DeserializeObject<ResponseDto<string>>(responseBody);
                        return responseDto;
                    }
                    else
                    {
                        return new ResponseDto<string> { IsSuccessFull = false, Data = response.StatusCode.ToString() + "  /  " + response.Content.ToString(), Message = ErrorsMessages.Faild, Status = "Api Response Status Code Is Not 200" };
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseDto<string> { IsSuccessFull = false, Data = ex.Message, Message = ErrorsMessages.InternalServerError, Status = "Exception" };
                }
            }
        }
        public async Task<ResponseDto<string>> GetRoomsList()
        {
            var authenticateResponse = await Authenticate();
            if (authenticateResponse.IsSuccessFull.Value == false)
            {
                return authenticateResponse;
            }
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticateResponse.Data);
                try
                {
                    var url = $"{middlewareAddress}Api/GetRoomsList?secretId={WebUtility.UrlEncode(SecretId)}";
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var responseDto = JsonConvert.DeserializeObject<ResponseDto<string>>(responseBody);
                        return responseDto;
                    }
                    else
                    {
                        return new ResponseDto<string> { IsSuccessFull = false, Data = response.StatusCode.ToString() + "  /  " + response.Content.ToString(), Message = ErrorsMessages.Faild, Status = "Api Response Status Code Is Not 200" };
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseDto<string> { IsSuccessFull = false, Data = ex.Message, Message = ErrorsMessages.InternalServerError, Status = "Exception" };
                }
            }
        }
        public async Task<ResponseDto<string>> GetSurgeryNamesList()
        {
            var authenticateResponse = await Authenticate();
            if (authenticateResponse.IsSuccessFull.Value == false)
            {
                return authenticateResponse;
            }
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticateResponse.Data);
                try
                {
                    var url = $"{middlewareAddress}Api/GetSurgeryNamesList?secretId={WebUtility.UrlEncode(SecretId)}";
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        var responseDto = JsonConvert.DeserializeObject<ResponseDto<string>>(responseBody);
                        return responseDto;
                    }
                    else
                    {
                        return new ResponseDto<string> { IsSuccessFull = false, Data = response.StatusCode.ToString() + "  /  " + response.Content.ToString(), Message = ErrorsMessages.Faild, Status = "Api Response Status Code Is Not 200" };
                    }
                }
                catch (Exception ex)
                {
                    return new ResponseDto<string> { IsSuccessFull = false, Data = ex.Message, Message = ErrorsMessages.InternalServerError, Status = "Exception" };
                }
            }
        }
    }
}
