using SurgeryRoomScheduler.Domain.Dtos.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Senders
{
    public interface ISender
    {

        //email
        void SendMailForSignUp(SendMailDto request);
        void SendMailForgetPassword(SendMailDto request);

        //sms
        Task<bool> LoginUserSendSmsAsync(string Mobile, string otp);
        Task<bool> ForgotPasswordSendSmsAsync(string Mobile, string otp);
        Task<bool> SendPatientCanecllationSmsAsync(string Mobile, string surgeryDate);
    }
}
