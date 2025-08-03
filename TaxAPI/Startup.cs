using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using TaxAPI.Helpers;
using TaxApp.BAL.Interface;
using TaxApp.BAL.Models;
using TaxApp.BAL.Services;
using Microsoft.AspNetCore.Rewrite;
using System.Reflection.Emit;
using TaxApp.BAL;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace TaxAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            Configuration = configuration;
            //SqlServerTypes.Utilities.LoadNativeAssemblies(env.ContentRootPath);

        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllersWithViews();
            services.AddCors(options =>
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
              {
                  o.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidIssuer = Configuration["AppSettings:Issuer"],
                      ValidAudience = Configuration["AppSettings:Audience"],
                      IssuerSigningKey = new SymmetricSecurityKey
                      (Encoding.UTF8.GetBytes(Configuration["AppSettings:Key"])),
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = false,
                      ValidateIssuerSigningKey = true
                  };
              });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("SuperAdmin", policy => policy.RequireRole("SuperAdmin"));
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 2147483648;
            });
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 2147483648;
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
                options.Limits.MaxRequestHeadersTotalSize = 100000; //100kb
                options.Limits.MaxRequestBufferSize = 2147483648; //10MB
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 2147483648;
            });
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 2147483648;
            });
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDeductorService, DeductorService>();
            services.AddScoped<IDeducteeService, DeducteeService>();
            services.AddScoped<IChallanService, ChallanService>();
            services.AddScoped<IEnumService, EnumServices>();
            services.AddScoped<IDeducteeEntryService, DeducteeEntryService>();
            services.AddScoped<IUploadFile, UploadFile>();
            services.AddScoped<IFormService, FormService>();
            services.AddScoped<ITDSDashboardService, TDSDashboardService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ISalaryDetailService, SalaryDetailService>();
            services.AddScoped<IFormValidationService, FormValidationService>();
            services.AddScoped<I24QValidationService, _24QValidationService>();
            services.AddScoped<I27QValidationService, _27QValidationService>();
            services.AddScoped<I27EQValidationService, _27EQValidationService>();
            services.AddScoped<I26QValidationService, _26QValidationService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSwaggerGen(options =>
            {
                // Define the Bearer Authentication scheme for Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
                });

                // Apply the security requirement globally
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });
            services.AddRazorPages();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor accessor, IConfiguration configuration, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {


            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(x => x
                       .AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();
            app.UseStatusCodePages(context =>
            {
                if (context.HttpContext.Response.StatusCode == 401)
                {
                    context.HttpContext.Response.ContentType = "application/json";
                    return context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        statusCode = 401,
                        message = "Unauthorized access. Please login."
                    }));
                }

                return Task.CompletedTask;
            });

            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseAuthorization();
            Helper.SetEnvironmentVariable(environment, configuration);

            var options = new RewriteOptions()
        .AddRedirectToHttpsPermanent();

            app.UseRewriter(options);
        }


    }
}
