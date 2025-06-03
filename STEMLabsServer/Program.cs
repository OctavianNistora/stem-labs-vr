using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Swashbuckle.AspNetCore.Filters;
using STEMLabsServer.Data;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services;
using STEMLabsServer.Services.Interfaces;
using STEMLabsServer.Shared;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MainDbContext>(options =>
{
    options.UseNpgsql(Environment.GetEnvironmentVariable("DATABASE_STRING") ?? 
                      throw new InvalidOperationException("DATABASE_STRING is not set."));
    options.UseSeeding((context, _) =>
    {
        var adminsExists = context.Set<User>().Any(user => user.UserRole == UserRole.Admin);
        if (!adminsExists)
        {
            context.Set<User>().Add(new User
            {
                Username = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? 
                           throw new InvalidOperationException("ADMIN_USERNAME is not set."),
                PasswordHashed = new PasswordHasher<User>().HashPassword(null!,
                    Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? 
                    throw new InvalidOperationException("ADMIN_PASSWORD is not set.")),
                Email = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? 
                        throw new InvalidOperationException("ADMIN_EMAIL is not set."),
                FirstName = "Preset",
                LastName = "Admin",
                PhoneNumber = Environment.GetEnvironmentVariable("ADMIN_PHONE") ?? 
                              throw new InvalidOperationException("ADMIN_PHONE is not set."),
                UserRole = UserRole.Admin,
                IsVerified = true, DateCreated = new DateTime(2025, 4, 20, 12, 0, 0, DateTimeKind.Utc)
            });
        }

        if (builder.Environment.IsDevelopment())
        {
            var student1 = context.Set<User>().FirstOrDefault(user => user.Username == "studentUsername1");
            if (student1 == null)
            {
                student1 = new User
                {
                    Username = "studentUsername1",
                    PasswordHashed = new PasswordHasher<User>().HashPassword(null!, "studentPassword1"),
                    Email = "student1.email@email-address.com",
                    FirstName = "Preset1",
                    LastName = "Student",
                    PhoneNumber = "1234567890",
                    UserRole = UserRole.Student,
                    IsVerified = true,
                    DateCreated = new DateTime(2025, 5, 3, 18, 23, 1, DateTimeKind.Utc)
                };
                context.Set<User>().Add(student1);
            }
            
            var student2 = context.Set<User>().FirstOrDefault(user => user.Username == "studentUsername2");
            if (student2 == null)
            {
                student2 = new User
                {
                    Username = "studentUsername2",
                    PasswordHashed = new PasswordHasher<User>().HashPassword(null!, "studentPassword2"),
                    Email = "student2.email@email-address.com",
                    FirstName = "Preset2",
                    LastName = "Student",
                    PhoneNumber = "0987654321",
                    UserRole = UserRole.Student,
                    IsVerified = true,
                    DateCreated = new DateTime(2025, 5, 5, 13, 20, 0, DateTimeKind.Utc)
                };
                context.Set<User>().Add(student2);
            }
            
            var student3 = context.Set<User>().FirstOrDefault(user => user.Username == "studentUsername3");
            if (student3 == null)
            {
                student3 = new User
                {
                    Username = "studentUsername3",
                    PasswordHashed = new PasswordHasher<User>().HashPassword(null!, "studentPassword3"),
                    Email = "student3.email@email-address.com",
                    FirstName = "Preset3",
                    LastName = "Student",
                    PhoneNumber = "1111111111",
                    UserRole = UserRole.Student,
                    IsVerified = true,
                    DateCreated = new DateTime(2025, 5, 7, 9, 15, 30, DateTimeKind.Utc)
                };
                context.Set<User>().Add(student3);
            }
            
            var professor1 = context.Set<User>().FirstOrDefault(user => user.Username == "professorUsername1");
            if (professor1 == null)
            {
                professor1 = new User
                {
                    Username = "professorUsername1",
                    PasswordHashed = new PasswordHasher<User>().HashPassword(null!, "professorPassword1"),
                    Email = "professor1.email@email-address.com",
                    FirstName = "Preset1",
                    LastName = "Professor",
                    PhoneNumber = "1234567890",
                    UserRole = UserRole.Professor,
                    IsVerified = true,
                    DateCreated = new DateTime(2025, 4, 20, 15, 44, 22, DateTimeKind.Utc)
                };
                context.Set<User>().Add(professor1);
            }
            
            
            var laboratory1 = context.Set<Laboratory>().FirstOrDefault(lab => lab.SceneId == 0);
            if (laboratory1 == null)
            {
                laboratory1 = new Laboratory
                {
                    Name = "PresetLaboratory1",
                    SceneId = 0,
                    CheckListStepCount = 3,
                };
                context.Set<Laboratory>().Add(laboratory1);
            }

            var laboratory1Step1 = context.Set<LaboratoryChecklistStep>()
                .FirstOrDefault(step => step.Laboratory == laboratory1 && step.StepNumber == 1);
            if (laboratory1Step1 == null)
            {
                laboratory1Step1 = new LaboratoryChecklistStep
                {
                    Laboratory = laboratory1,
                    Version = 1,
                    StepNumber = 1,
                    Statement = "Preset step 1 for laboratory 1"
                };
                context.Set<LaboratoryChecklistStep>().Add(laboratory1Step1);
            }
            
            var laboratory1Step2 = context.Set<LaboratoryChecklistStep>()
                .FirstOrDefault(step => step.Laboratory == laboratory1 && step.StepNumber == 2);
            if (laboratory1Step2 == null)
            {
                laboratory1Step2 = new LaboratoryChecklistStep
                {
                    Laboratory = laboratory1,
                    Version = 1,
                    StepNumber = 2,
                    Statement = "Preset step 2 for laboratory 1"
                };
                context.Set<LaboratoryChecklistStep>().Add(laboratory1Step2);
            }
            
            var laboratory1Step3 = context.Set<LaboratoryChecklistStep>()
                .FirstOrDefault(step => step.Laboratory == laboratory1 && step.StepNumber == 3);
            if (laboratory1Step3 == null)
            {
                laboratory1Step3 = new LaboratoryChecklistStep
                {
                    Laboratory = laboratory1,
                    Version = 1,
                    StepNumber = 3,
                    Statement = "Preset step 3 for laboratory 1"
                };
                context.Set<LaboratoryChecklistStep>().Add(laboratory1Step3);
            }
            
            
            var laboratorySession1 = context.Set<LaboratorySession>()
                .FirstOrDefault(session =>
                    session.Laboratory == laboratory1 &&
                    session.CreatedAt == new DateTime(2025, 5, 10, 10, 0, 0, DateTimeKind.Utc));
            if (laboratorySession1 == null)
            {
                laboratorySession1 = new LaboratorySession
                {
                    CreatedBy = professor1,
                    Laboratory = laboratory1,
                    InviteCode = "inviteCode1",
                    CreatedAt = new DateTime(2025, 5, 10, 10, 0, 0, DateTimeKind.Utc),
                };
                context.Set<LaboratorySession>().Add(laboratorySession1);
            }

            var laboratorySession2 = context.Set<LaboratorySession>()
                .FirstOrDefault(session =>
                    session.Laboratory == laboratory1 &&
                    session.CreatedAt == new DateTime(2025, 5, 11, 12, 12, 12, DateTimeKind.Utc));
            if (laboratorySession2 == null)
            {
                laboratorySession2 = new LaboratorySession
                {
                    CreatedBy = professor1,
                    Laboratory = laboratory1,
                    InviteCode = "inviteCode2",
                    CreatedAt = new DateTime(2025, 5, 11, 12, 12, 12, DateTimeKind.Utc),
                };
                context.Set<LaboratorySession>().Add(laboratorySession2);
            }
            
            
            var laboratoryReport1 = context.Set<StudentLaboratoryReport>()
                .FirstOrDefault(report => report.LaboratorySession == laboratorySession1 && report.Student == student1);
            if (laboratoryReport1 == null)
            {
                laboratoryReport1 = new StudentLaboratoryReport
                {
                    LaboratorySession = laboratorySession1,
                    Student = student1,
                    ObservationsImageLink = null,
                    CreatedAt = new DateTime(2025, 5, 10, 11, 0, 0, DateTimeKind.Utc),
                };
                context.Set<StudentLaboratoryReport>().Add(laboratoryReport1);
            }
            
            var laboratoryReport1Step1 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport1 && step.LaboratoryChecklistStep == laboratory1Step1);
            if (laboratoryReport1Step1 == null)
            {
                laboratoryReport1Step1 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport1,
                    LaboratoryChecklistStep = laboratory1Step1,
                    IsCompleted = false,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport1Step1);
            }
            
            var laboratoryReport1Step2 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport1 && step.LaboratoryChecklistStep == laboratory1Step2);
            if (laboratoryReport1Step2 == null)
            {
                laboratoryReport1Step2 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport1,
                    LaboratoryChecklistStep = laboratory1Step2,
                    IsCompleted = false,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport1Step2);
            }
            
            var laboratoryReport1Step3 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport1 && step.LaboratoryChecklistStep == laboratory1Step3);
            if (laboratoryReport1Step3 == null)
            {
                laboratoryReport1Step3 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport1,
                    LaboratoryChecklistStep = laboratory1Step3,
                    IsCompleted = false,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport1Step3);
            }
            
            var laboratoryReport2 = context.Set<StudentLaboratoryReport>()
                .FirstOrDefault(report => report.LaboratorySession == laboratorySession1 && report.Student == student2);
            if (laboratoryReport2 == null)
            {
                laboratoryReport2 = new StudentLaboratoryReport
                {
                    LaboratorySession = laboratorySession1,
                    Student = student2,
                    ObservationsImageLink = "https://upload.wikimedia.org/wikipedia/commons/thumb/7/7d/Microsoft_.NET_logo.svg/800px-Microsoft_.NET_logo.svg.png",
                    CreatedAt = new DateTime(2025, 5, 10, 11, 30, 0, DateTimeKind.Utc),
                };
                context.Set<StudentLaboratoryReport>().Add(laboratoryReport2);
            }
            
            var laboratoryReport2Step1 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport2 && step.LaboratoryChecklistStep == laboratory1Step1);
            if (laboratoryReport2Step1 == null)
            {
                laboratoryReport2Step1 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport2,
                    LaboratoryChecklistStep = laboratory1Step1,
                    IsCompleted = true,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport2Step1);
            }
            
            var laboratoryReport2Step2 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport2 && step.LaboratoryChecklistStep == laboratory1Step2);
            if (laboratoryReport2Step2 == null)
            {
                laboratoryReport2Step2 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport2,
                    LaboratoryChecklistStep = laboratory1Step2,
                    IsCompleted = true,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport2Step2);
            }
            
            var laboratoryReport2Step3 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport2 && step.LaboratoryChecklistStep == laboratory1Step3);
            if (laboratoryReport2Step3 == null)
            {
                laboratoryReport2Step3 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport2,
                    LaboratoryChecklistStep = laboratory1Step3,
                    IsCompleted = true,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport2Step3);
            }
            
            var laboratoryReport3 = context.Set<StudentLaboratoryReport>()
                .FirstOrDefault(report => report.LaboratorySession == laboratorySession2 && report.Student == student1);
            if (laboratoryReport3 == null)
            {
                laboratoryReport3 = new StudentLaboratoryReport
                {
                    LaboratorySession = laboratorySession2,
                    Student = student1,
                    ObservationsImageLink = "https://upload.wikimedia.org/wikipedia/commons/c/c4/Unity_2021.svg",
                    CreatedAt = new DateTime(2025, 5, 11, 12, 30, 0, DateTimeKind.Utc),
                };
                context.Set<StudentLaboratoryReport>().Add(laboratoryReport3);
            }
            
            var laboratoryReport3Step1 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport3 && step.LaboratoryChecklistStep == laboratory1Step1);
            if (laboratoryReport3Step1 == null)
            {
                laboratoryReport3Step1 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport3,
                    LaboratoryChecklistStep = laboratory1Step1,
                    IsCompleted = true,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport3Step1);
            }
            
            var laboratoryReport3Step2 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport3 && step.LaboratoryChecklistStep == laboratory1Step2);
            if (laboratoryReport3Step2 == null)
            {
                laboratoryReport3Step2 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport3,
                    LaboratoryChecklistStep = laboratory1Step2,
                    IsCompleted = false,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport3Step2);
            }
            
            var laboratoryReport3Step3 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport3 && step.LaboratoryChecklistStep == laboratory1Step3);
            if (laboratoryReport3Step3 == null)
            {
                laboratoryReport3Step3 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport3,
                    LaboratoryChecklistStep = laboratory1Step3,
                    IsCompleted = false,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport3Step3);
            }
            
            var laboratoryReport4 = context.Set<StudentLaboratoryReport>()
                .FirstOrDefault(report => report.LaboratorySession == laboratorySession2 && report.Student == student2);
            if (laboratoryReport4 == null)
            {
                laboratoryReport4 = new StudentLaboratoryReport
                {
                    LaboratorySession = laboratorySession2,
                    Student = student3,
                    ObservationsImageLink = null,
                    CreatedAt = new DateTime(2025, 5, 11, 12, 45, 0, DateTimeKind.Utc),
                };
                context.Set<StudentLaboratoryReport>().Add(laboratoryReport4);
            }
            
            var laboratoryReport4Step1 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport4 && step.LaboratoryChecklistStep == laboratory1Step1);
            if (laboratoryReport4Step1 == null)
            {
                laboratoryReport4Step1 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport4,
                    LaboratoryChecklistStep = laboratory1Step1,
                    IsCompleted = true,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport4Step1);
            }
            
            var laboratoryReport4Step2 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport4 && step.LaboratoryChecklistStep == laboratory1Step2);
            if (laboratoryReport4Step2 == null)
            {
                laboratoryReport4Step2 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport4,
                    LaboratoryChecklistStep = laboratory1Step2,
                    IsCompleted = false,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport4Step2);
            }
            
            var laboratoryReport4Step3 = context.Set<StudentLaboratoryReportStep>()
                .FirstOrDefault(step => step.StudentLaboratoryReport == laboratoryReport4 && step.LaboratoryChecklistStep == laboratory1Step3);
            if (laboratoryReport4Step3 == null)
            {
                laboratoryReport4Step3 = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport4,
                    LaboratoryChecklistStep = laboratory1Step3,
                    IsCompleted = true,
                };
                context.Set<StudentLaboratoryReportStep>().Add(laboratoryReport4Step3);
            }
        }
        
        context.SaveChanges();
    });
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILaboratoryService, LaboratoryService>();
builder.Services.AddScoped<IRecoveryService, RecoveryService>();
builder.Services.AddScoped<ILaboratoryReportService, LaboratoryReportService>();
builder.Services.AddScoped<ILaboratorySessionService, LaboratorySessionService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.AddControllers();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                          throw new InvalidOperationException("JWT_ISSUER is not set."),
            ValidateAudience = true,
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? 
                            throw new InvalidOperationException("JWT_AUDIENCE is not set."),
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_TOKEN_KEY") ??
                                       throw new InvalidOperationException())),
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "JWT Authorization",
        Description = "Enter your JWT token in this fiedld",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
    });
    
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            }, []
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();