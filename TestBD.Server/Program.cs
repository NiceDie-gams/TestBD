using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TestBD.Server.Data;
using TestBD.Server.Dtos;
using TestBD.Server.Models;
using TestBD.Server.Profiles;
using TestBD.Server.Repository;
using TestBD.Server.Services;
using System.Text.Json.Serialization.Metadata;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;

//var builder = WebApplication.CreateBuilder(args);

//var jwtSettings = builder.Configuration.GetSection("Jwt");
//var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key не настроен"));

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(key),
//            ClockSkew = TimeSpan.Zero,
//            RoleClaimType = "role",
//            NameClaimType = "login"
//        };
//    });

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
//    options.AddPolicy("EmployeeOrAdmin", policy => policy.RequireRole("employee", "admin"));
//    options.AddPolicy("UserOnly", policy => policy.RequireRole("user"));
//});
var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key не настроен"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Для разработки
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero,
        // 🔴 ВАЖНО: Указываем правильные Claim Types
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };

    // 🔴 ОТЛАДКА: Добавляем обработчики событий для отладки
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            var claims = context.Principal?.Claims;
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("admin"));
    options.AddPolicy("EmployeeOrAdmin", policy =>
        policy.RequireRole("employee", "admin"));
    options.AddPolicy("UserOnly", policy =>
        policy.RequireRole("user"));
    options.AddPolicy("UserOrAdmin", policy =>
        policy.RequireRole("user", "admin"));
});


// Регистрация репозиториев
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<RepositoryPatient>();
builder.Services.AddScoped<ServiceRepository>();
builder.Services.AddScoped<EmployeeRepository>();
builder.Services.AddScoped<AppointmentRepository>();
builder.Services.AddScoped<VisitHistoryRepository>();
builder.Services.AddScoped<DoctorScheduleRepository>();
builder.Services.AddScoped<ProvidedServiceRepository>();
builder.Services.AddScoped<PaymentRepository>();

// Регистрация сервисов
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UslugiService>();
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<VisitHistoryService>();
builder.Services.AddScoped<DoctorScheduleService>();
builder.Services.AddScoped<ProvidedUslugaService>();
builder.Services.AddScoped<PaymentService>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.CreateMap<Patient, PatientDto>()
    .ForMember(x => x.PatientId, map => map.MapFrom(src => src.PatientId ?? Guid.NewGuid())).ReverseMap();
    cfg.CreateMap<Service, ServiceDto>();
    cfg.CreateMap<Employee, EmployeeDto>().ReverseMap();
    cfg.AddProfile<AppointmentProfile>();
    cfg.CreateMap<VisitHistory, VisitHistoryDto>(MemberList.None)
        .ForMember(x => x.EmployeeFio, map => map.MapFrom(x => x.Appointment.Schedule.Doctor.EmployeeFio));
    cfg.CreateMap<DoctorSchedule, DoctorScheduleDto>(MemberList.None)
        .ForMember(x => x.EmployeeId, map => map.MapFrom(x => x.Doctor.EmployeeId));
    cfg.CreateMap<Providedservice, ProvidedServiceDto>(MemberList.None).ReverseMap();
    cfg.CreateMap<Appointment, AppointmentRequestDto>(MemberList.None)
        .ReverseMap();
    cfg.CreateMap<VisitHistory, VisitHistoryRequestDto>(MemberList.None).ReverseMap();
    cfg.CreateMap<Payment, PaymentDto>(MemberList.None)
        .ForMember(x => x.PatientFio, map => map.MapFrom(src => src.ProvidedService.Appointment.Patient.PatientFio)); 
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());

    });
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "My API",
        Version = "v1",
        Description = "API для медицинской системы"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDev",
        policy =>
        {
            policy.WithOrigins(
                    "https://localhost:58949",    // Порт Vite
                    "http://localhost:58949",     // HTTP версия
                    "https://localhost:5173",     // Альтернативный порт Vite
                    "http://localhost:5173"       // HTTP альтернатива
                )
                .AllowAnyMethod()                  // Разрешить все HTTP методы
                .AllowAnyHeader()                  // Разрешить все заголовки
                .AllowCredentials()                // Разрешить куки/авторизацию
                .SetPreflightMaxAge(TimeSpan.FromHours(1)); // Кэшировать preflight запросы
        });
});


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

//app.UseCors("AllowAllForDevelopment");

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
