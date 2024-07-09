using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SurgeryRoomScheduler.Domain.Dtos.Common;
using SurgeryRoomScheduler.Domain.Dtos.Common.ResponseModel;
using SurgeryRoomScheduler.Domain.Entities.Account;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Application.Utilities
{
    public static class UtilityManager
    {
        public static bool IsMobile(string input)
        {
            string mobilePattern = @"^09\d{9}$";
            return Regex.IsMatch(input, mobilePattern);
        }

        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            ?.GetName() ?? enumValue.ToString();
        }
        public static bool IsValidNationalCode(string input)
        {
            string pattern = @"^[0-9]{10}$";
            return Regex.IsMatch(input, pattern);
        }
        public static string OtpGenrator()
        {
            int length = 6;
            // Define characters allowed in the OTP
            const string allowedChars = "0123456789";

            // Create a random number generator
            Random random = new Random();

            // Use StringBuilder to efficiently build the OTP
            StringBuilder otpBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                // Generate a random index to select a character from allowedChars
                int index = random.Next(0, allowedChars.Length);

                // Append the selected character to the OTP
                otpBuilder.Append(allowedChars[index]);
            }

            return otpBuilder.ToString();
        }

        public static Guid GetCurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            if (!httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                //return new Guid("14F942EB-161B-4F28-9F04-B5A96A96D19E");// سوپروایزر
                //return new Guid("55bded22-d3f8-4b04-5440-08dc9faa1a8d");// دکتر
                //return new Guid("E3597C08-046F-43E5-B85B-CA43F54066F6");// مدارک پزشکی
                return Guid.Empty;
            }
            var user = httpContextAccessor.HttpContext.User;
            return Guid.Parse(user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
        }

        public static string EncodePasswordMd5(string pass)
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;
            //Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)   
            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(pass);
            encodedBytes = md5.ComputeHash(originalBytes);
            //Convert encoded bytes back to a 'readable' string   
            return BitConverter.ToString(encodedBytes);
        }

        public static string ConvertGregorianDateTimeToPersianDate(DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            int year = persianCalendar.GetYear(date);
            int month = persianCalendar.GetMonth(date);
            int day = persianCalendar.GetDayOfMonth(date);

            // Format the date as "yyyy/MM/dd"
            return $"{year:0000}/{month:00}/{day:00}";
        }

        public static string GregorianDateTimeToPersianDate(DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            int year = persianCalendar.GetYear(date);
            int month = persianCalendar.GetMonth(date);
            int day = persianCalendar.GetDayOfMonth(date);
            int hour = persianCalendar.GetHour(date);
            int minute = persianCalendar.GetMinute(date);
            int second = persianCalendar.GetSecond(date);

            // Format the date as "yyyy/MM/dd HH:mm:ss"
            return $"{year:0000}/{month:00}/{day:00} {hour:00}:{minute:00}:{second:00}";
        }
        public static string GregorianDateTimeToPersianDateOnly(DateOnly date)
        {
            // Convert DateOnly to DateTime using midnight time
            DateTime dateTime = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Unspecified);

            PersianCalendar persianCalendar = new PersianCalendar();
            int year = persianCalendar.GetYear(dateTime);
            int month = persianCalendar.GetMonth(dateTime);
            int day = persianCalendar.GetDayOfMonth(dateTime);

            // Format the date as "yyyy/MM/dd"
            return $"{year:0000}/{month:00}/{day:00}";
        }


        public static string GregorianDateTimeToPersianDateDashType(DateTime date)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            int year = persianCalendar.GetYear(date);
            int month = persianCalendar.GetMonth(date);
            int day = persianCalendar.GetDayOfMonth(date);
            int hour = persianCalendar.GetHour(date);
            int minute = persianCalendar.GetMinute(date);
            int second = persianCalendar.GetSecond(date);

            // Format the date as "yyyy/MM/dd HH:mm:ss"
            return $"{year:0000}-{month:00}";
        }

        public static DateTime GetLastDayOfPersianMonth(int year, int month)
        {
            // Define the Persian calendar
            PersianCalendar persianCalendar = new PersianCalendar();

            // Determine the last day of the month
            int lastDay;
            if (month < 1 || month > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
            }

            if (month <= 6)
            {
                lastDay = 31;
            }
            else if (month <= 11)
            {
                lastDay = 30;
            }
            else
            {
                // The last month (Esfand) can be 29 or 30 days depending on whether it's a leap year
                lastDay = persianCalendar.IsLeapYear(year) ? 30 : 29;
            }

            // Convert the Persian date to Gregorian date
            DateTime lastDayOfMonth = persianCalendar.ToDateTime(year, month, lastDay, 0, 0, 0, 0);

            return lastDayOfMonth;
        }

        public static DateTime ConvertPersianToGregorian(string persianDate)
        {
            string[] parts = persianDate.Split('-');
            int year = int.Parse(parts[0]);
            int month = int.Parse(parts[1]);
            int day = int.Parse(parts[2]);

            PersianCalendar pc = new PersianCalendar();
            DateTime gregorianDate = pc.ToDateTime(year, month, day, 0, 0, 0, 0);

            return gregorianDate;
        }


        public static string ConvertGregorianToPersian(string gregorianDateString)
        {
            if (gregorianDateString.IsNullOrEmpty())
            {
                gregorianDateString = DateTime.Now.ToString("yyyyMMdd");
            }
            DateTime gregorianDate;

            // Try to parse the input string into a DateTime object
            if (DateTime.TryParseExact(gregorianDateString, "yyyyMMdd",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out gregorianDate))
            {
                PersianCalendar persianCalendar = new PersianCalendar();

                int persianYear = persianCalendar.GetYear(gregorianDate);
                int persianMonth = persianCalendar.GetMonth(gregorianDate);
                int persianDay = persianCalendar.GetDayOfMonth(gregorianDate);

                return $"{persianYear:0000}/{persianMonth:00}/{persianDay:00}";
            }
            else
            {
                throw new ArgumentException("The input string is not in the correct format (yyyyMMdd).");
            }
        }

        private static readonly string[] PersianMonthNames =
        {
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

        private static readonly string[] PersianDayOfWeekNames =
        {
            "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنج‌شنبه", "جمعه", "شنبه"
        };

        public static GetPersianDateDto ConvertGregorianToPersianWithDetails(string gregorianDateString)
        {
            DateTime gregorianDate;
            if (gregorianDateString.IsNullOrEmpty())
            {
                gregorianDateString = DateTime.Now.ToString("yyyyMMdd");
            }
            // Try to parse the input string into a DateTime object
            if (DateTime.TryParseExact(gregorianDateString, "yyyyMMdd",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out gregorianDate))
            {
                PersianCalendar persianCalendar = new PersianCalendar();

                int persianYear = persianCalendar.GetYear(gregorianDate);
                int persianMonth = persianCalendar.GetMonth(gregorianDate);
                int persianDay = persianCalendar.GetDayOfMonth(gregorianDate);

                string persianDate = $"{persianYear:0000}/{persianMonth:00}/{persianDay:00}";
                string persianMonthName = PersianMonthNames[persianMonth - 1];
                string persianDayOfWeekName = PersianDayOfWeekNames[(int)gregorianDate.DayOfWeek];

                return (new GetPersianDateDto { PersianDate = persianDate, PersianDayOfWeek = persianMonthName, PersianMonth = persianDayOfWeekName });
            }
            else
            {
                throw new ArgumentException("The input string is not in the correct format (yyyyMMdd).");
            }
        }
        public static GetPersianDateDto GetPersianDateWithDetails(string persianDate)
        {
            // Try to parse the input string into a DateTime object
            if (DateTime.TryParseExact(persianDate, "yyyyMMdd",
                                       CultureInfo.InvariantCulture,
                                       DateTimeStyles.None,
                                       out DateTime gregorianDate))
            {
                PersianCalendar persianCalendar = new PersianCalendar();

                int persianYear = persianCalendar.GetYear(gregorianDate);
                int persianMonth = persianCalendar.GetMonth(gregorianDate);
                int persianDay = persianCalendar.GetDayOfMonth(gregorianDate);

                string formattedPersianDate = $"{persianYear:0000}/{persianMonth:00}/{persianDay:00}";
                string persianMonthName = PersianMonthNames[persianMonth - 1];
                string persianDayOfWeekName = PersianDayOfWeekNames[(int)gregorianDate.DayOfWeek];

                return new GetPersianDateDto
                {
                    PersianDate = formattedPersianDate,
                    PersianMonth = persianMonthName,
                    PersianDayOfWeek = persianDayOfWeekName
                };
            }
            else
            {
                throw new ArgumentException("The input string is not in the correct format (yyyyMMdd).");
            }
        }


        public static List<PersianDayInfoDto> GetDaysOfPersianMonth(int year, int month)
        {
            // Define the Persian calendar
            PersianCalendar persianCalendar = new PersianCalendar();
            // Define the Gregorian calendar
            GregorianCalendar gregorianCalendar = new GregorianCalendar();

            // List to hold the days of the month
            List<PersianDayInfoDto> daysOfMonth = new List<PersianDayInfoDto>();

            // Determine the number of days in the month
            int lastDay;
            if (month <= 6)
            {
                lastDay = 31;
            }
            else if (month <= 11)
            {
                lastDay = 30;
            }
            else
            {
                lastDay = persianCalendar.IsLeapYear(year) ? 30 : 29;
            }

            DateTime today = DateTime.Now;
            DateTime threeDaysLater = today.AddDays(3);

            for (int day = 1; day <= lastDay; day++)
            {
                DateTime miladiDate = persianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
                string shamsiDate = $"{year:0000}/{month:00}/{day:00}";

                int gregorianDayOfWeek = (int)miladiDate.DayOfWeek;
                string persianWeekDay = PersianDayOfWeekNames[gregorianDayOfWeek];
                bool isEnable = !(miladiDate < today || miladiDate < threeDaysLater);

                daysOfMonth.Add(new PersianDayInfoDto
                {
                    Day = day.ToString("00"),
                    DayOfTheWeek = persianWeekDay,
                    ShamsiDate = shamsiDate,
                    MiladiDate = miladiDate,
                    IsEnable = isEnable
                });
            }

            return daysOfMonth;
        }

    }
}
