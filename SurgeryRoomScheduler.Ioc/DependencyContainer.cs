using Microsoft.Extensions.DependencyInjection;
using SurgeryRoomScheduler.Domain.Interfaces;
using SurgeryRoomScheduler.Domain.Interfaces.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SurgeryRoomScheduler.Data.Repositories;
using SurgeryRoomScheduler.Data.Repositories.Users;
using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Application.Services.Implementations;
using SurgeryRoomScheduler.Application.Security;
using SurgeryRoomScheduler.Application.Senders;
using SurgeryRoomScheduler.Application.Jobs.Interfaces;
using SurgeryRoomScheduler.Application.Jobs.Implementations;


namespace SurgeryRoomScheduler.Ioc
{
    public static class DependencyContainer
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITimingRepository, TimingRepository>();
            services.AddScoped<IDoctorRepository, DoctorRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();



            services.AddScoped<ITimingService, TimingService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMedicalDataService, MedicalDataService>();
            services.AddScoped<IReservationService, ReservationService>();


            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ISender, Sender>();
            services.AddScoped<ILogService,  LogService>();
            services.AddScoped<IJobs, Jobs>();
            services.AddScoped<IExternalService, ExternalService>();
        }
    }
}
