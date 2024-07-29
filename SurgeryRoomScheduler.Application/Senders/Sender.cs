using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Farapayamak;
using Microsoft.Extensions.Configuration;
using SurgeryRoomScheduler.Domain.Dtos.Sender;


namespace SurgeryRoomScheduler.Application.Senders
{
    public class Sender : ISender
    {
        public static string Username = "miladhospital";
        public static string Password = "S1394#Desk";

        private readonly IConfiguration _configuration;
        public Sender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> ForgotPasswordSendSmsAsync(string Mobile, string otp)
        {
            RestClient restClient = new RestClient(Username, Password);
            restClient.BaseServiceNumber(otp, Mobile, 224492);
            return true;
        }

        public async Task<bool> LoginUserSendSmsAsync(string Mobile, string otp)
        {
            try
            {
                RestClient restClient = new RestClient(Username, Password);
                restClient.BaseServiceNumber(otp, Mobile, 223858);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }

        public void SendMailForgetPassword(SendMailDto request)
        {
            try
            {
                var mail = new MailMessage();

                var SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("Email", "Tittle");
                mail.To.Add(request.Email);
                string password = _configuration["Password"];
                mail.Subject = "Forget Password";
                mail.Body = "Body";
                SmtpServer.EnableSsl = true;
                mail.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("Email", password);
                SmtpServer.Send(mail);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void SendMailForSignUp(SendMailDto request)
        {
            try
            {
                var mail = new MailMessage();
                var SmtpServer = new SmtpClient("smtp.gmail.com");
                mail.From = new MailAddress("Email", "Tittle");
                mail.To.Add(request.Email);
                string password = _configuration["Password"];
                mail.Subject = "Sign up Verification Code";
                mail.Body = " Body";
                SmtpServer.EnableSsl = true;
                mail.IsBodyHtml = true;
                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("Email", password);
                SmtpServer.Send(mail);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SendPatientCanecllationSmsAsync(string Mobile, string surgeryDate)
        {
            try
            {
                RestClient restClient = new RestClient(Username, Password);
                restClient.BaseServiceNumber(surgeryDate, Mobile, 236483);
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }
        }
    }
}
