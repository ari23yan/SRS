using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SurgeryRoomScheduler.Application;
using SurgeryRoomScheduler.Application.Jobs;
using SurgeryRoomScheduler.Application.Jobs.Interfaces;
using SurgeryRoomScheduler.Application.Services.Implementations;
using SurgeryRoomScheduler.Application.Services.Interfaces;
using SurgeryRoomScheduler.Data.Context;
using SurgeryRoomScheduler.Ioc;
using SurgeryRoomScheduler.Presentation;
using System.Configuration;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder =>
        {
            builder.WithOrigins("https://localhost:3000", "https://localhost:3001")
                .AllowAnyMethod()
                .AllowAnyHeader() // Allow any header
                .AllowCredentials();
        });
});

builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
    {
        sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "Common");
    }
));


// Job Conf --- HangFire

builder.Services.AddHangfire(config =>
                   config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))
               );
builder.Services.AddHangfireServer();



var secrectKey = builder.Configuration.GetSection("Authentication:IssuerSigningKey");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "your_issuer",
        ValidAudience = "your_audience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secrectKey.Value)),
    };
});

builder.Services.AddDependencies();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpContextAccessor();

// Add controllers
builder.Services.AddControllersWithViews()
    .AddFluentValidation(options =>
    {
        options.ImplicitlyValidateChildProperties = true;
        options.ImplicitlyValidateRootCollectionElements = true;
        options.RegisterValidatorsFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
    })
     .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()))
    .AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddControllers(x =>
{
    x.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Milad Surgery Room", Version = "v1", Description = "Milad Surgery Room Rest Api Services - 2024 " });
    c.EnableAnnotations();

    //c.EnableAnnotations();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();
app.UseCors("AllowOrigin");
app.UseHangfireDashboard("/hangfire");

app.UseWhen(context => !context.Request.Path.StartsWithSegments("/hangfire"), appBuilder =>
{
    // Add authentication middleware here, if any
    appBuilder.UseAuthentication();
    appBuilder.UseAuthorization();
});

//Jobs Configuration
RecurringJob.AddOrUpdate<IJobs>(job => job.GetDoctorsListJob(), Cron.Daily);
RecurringJob.AddOrUpdate<IJobs>(job => job.GetRoomListJob(), Cron.Daily);
RecurringJob.AddOrUpdate<IJobs>(job => job.GetInsuranceListJob(), Cron.Daily);
RecurringJob.AddOrUpdate<IJobs>(job => job.GetSurgeryNamesListJob(), Cron.Daily);
RecurringJob.AddOrUpdate<IJobs>(job => job.GetDoctorsAssignedRooms(), Cron.Daily);
RecurringJob.AddOrUpdate<IJobs>(job => job.ExteraTimingJob(), Cron.Daily);

app.UseHangfireServer();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();
