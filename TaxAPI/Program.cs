using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using TaxAPI.Helpers;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Services;
//using DinkToPdf;
//using DinkToPdf.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("TaxAppPolicy",
                   builder =>
                   {
                       builder
                           .WithOrigins("https://taxvahan.site/", "http://localhost:3001")
                           .AllowCredentials()
                           .AllowAnyHeader()
                           .AllowAnyMethod();
                   });
});
IConfiguration configuration = builder.Configuration;
// Add configuration sources (defaults to appsettings.json and environment variables)
//builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
//builder.Configuration.AddEnvironmentVariables();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes("MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAErOdtape1YLgNJgTUg5OJaNDOP9zlCQvRC9grT0/LOsQG/Ci8fw2GUMmt7uhT7H9ye10ltk5nWWXdX5Bfb8OlzA==")),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = "https://taxapp.com/", // Replace with your issuer
        ValidAudience = "https://taxapp.com/", // Replace with your audience,
    };
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
    options.Limits.MaxRequestHeadersTotalSize = 100000; //100kb
    options.Limits.MaxRequestBufferSize = 2147483648; //10MB
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SuperAdmin", policy => policy.RequireRole("SuperAdmin"));
});
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 2147483648;
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2147483648;
});
builder.Services.AddDbContext<DbContext>(ServiceLifetime.Transient);
builder.Services.AddControllers();
// configure strongly typed settings object
//builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
//builder.Services.Configure<Smtp>(builder.Configuration.GetSection("Smtp"));
//builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
#region Dependency resolution
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDeductorService, DeductorService>();
builder.Services.AddScoped<IDeducteeService, DeducteeService>();
builder.Services.AddScoped<IChallanService, ChallanService>();
builder.Services.AddScoped<IEnumService, EnumServices>();
builder.Services.AddScoped<IDeducteeEntryService, DeducteeEntryService>();
builder.Services.AddScoped<IUploadFile, UploadFile>();
builder.Services.AddScoped<IFormService, FormService>();
builder.Services.AddScoped<ITDSDashboardService, TDSDashboardService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ISalaryDetailService, SalaryDetailService>();
builder.Services.AddScoped<IFormValidationService, FormValidationService>();
builder.Services.AddScoped<I24QValidationService, _24QValidationService>();
builder.Services.AddScoped<I27QValidationService, _27QValidationService>();
builder.Services.AddScoped<I27EQValidationService, _27EQValidationService>();
builder.Services.AddScoped<I26QValidationService, _26QValidationService>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<ITracesActivitiesService, TracesActivitiesService>();
builder.Services.AddScoped<IDdoDetailsService, DdoDetailsService>();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//app.UseSwagger();
//app.UseSwaggerUI();
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "ITRTemplate")),
    RequestPath = "/ITRTemplate"
});
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("TaxAppPolicy");
app.MapControllers();

app.Run();
