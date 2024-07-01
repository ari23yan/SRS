using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel
{
    //public class ErrorsMessages
    //{
    //    public static string FaildLogin = "Error, please try again";
    //    public static string BadApiResponse = "Error , Api Response Is Not 200";
    //    public static string SuccessLogin = "successful";
    //    public static string UserORPasswrodIsWrong = "Username or password is incorrect";
    //    public static string NotAuthenticated = "User not authenticated";
    //    public static string Authenticated = "User authenticated";
    //    public static string NotFound = "Not found";
    //    public static string SuccessRegister = "Registration successful";
    //    public static string UserNotfound = "User not found, please try again";
    //    public static string NotActive = "The user account is inactive";
    //    public static string PassswordAndConfrimPassswordIsnotEqueal = "Password and confirm password do not match, please try again";
    //    public static string UserExist = "A user with these credentials already exists";
    //    public static string Exist = "Record with these credentials already exists";
    //    public static string NullInputs = "Input values are empty";
    //    public static string Success = "Operation successful";
    //    public static string Faild = "Operation failed";
    //    public static string OtpIncorrect = "Incorrect OTP";
    //    public static string EmailSend = "Verification code has been sent to your email.";
    //    public static string PermissionDenied = "Permission Denied";
    //    public static string InternalServerError = "Opps,Internal Server Error";
    //    public static string PhoneNumberAlreadyExist = "PhoneNumber Alredy Exist in Database ";
    //    public static string UsernameAlreadyExist = "Username Alredy Exist in Database ";
    //    public static string SmsPanelNotResponding = "Sms Not Sended ,Farapayamk Internal Server Error";
    //}


    public class ErrorsMessages
    {
        public static string FaildLogin = "خطا، لطفاً دوباره تلاش کنید";
        public static string BadApiResponse = "خطا، پاسخ API نامعتبر است";
        public static string SuccessLogin = "ورود موفقیت‌آمیز";
        public static string UserORPasswrodIsWrong = "نام کاربری یا رمز عبور اشتباه است";
        public static string NotAuthenticated = "کاربر احراز هویت نشده است";
        public static string Authenticated = "کاربر احراز هویت شده است";
        public static string NotFound = "موردی یافت نشد";
        public static string SuccessRegister = "ثبت‌نام موفقیت‌آمیز بود";
        public static string UserNotfound = "کاربر یافت نشد، لطفاً دوباره تلاش کنید";
        public static string NotActive = "حساب کاربری غیرفعال است";
        public static string PassswordAndConfrimPassswordIsnotEqueal = "رمز عبور و تکرار آن مطابقت ندارند، لطفاً دوباره تلاش کنید";
        public static string UserExist = "کاربری با این مشخصات قبلاً وجود دارد";
        public static string Exist = "رکوردی با این مشخصات قبلاً وجود دارد";
        public static string NullInputs = "مقادیر ورودی خالی هستند";
        public static string Success = "عملیات موفقیت‌آمیز بود";
        public static string Faild = "عملیات ناموفق بود";
        public static string OtpIncorrect = "کد تایید اشتباه است";
        public static string EmailSend = "کد تایید به ایمیل شما ارسال شد";
        public static string PermissionDenied = "دسترسی رد شد";
        public static string InternalServerError = "اوه، خطای سرور داخلی";
        public static string PhoneNumberAlreadyExist = "شماره تلفن در پایگاه داده موجود است";
        public static string UsernameAlreadyExist = "نام کاربری در پایگاه داده موجود است";
        public static string SmsPanelNotResponding = "پیامک ارسال نشد، خطای سرور داخلی فراپیامک";
    }
}
