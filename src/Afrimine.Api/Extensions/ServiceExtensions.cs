using Afrimine.Migrations;
using Afrimine.Model.Entities;
using Afrimine.Repository;
using Afrimine.Services.BL.Implementation;
using Afrimine.Services.BL.Interfaces;
using Afrimine.Shared.Configs;
using Afrimine.Shared.Contract;
using Afrimine.Shared.ExternalServices;
using Asp.Versioning;
using Hangfire;
using Hangfire.PostgreSql;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace Afrimine.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void RegisterServices(this IServiceCollection services,
                                            IConfiguration configuration)
        {
            services.AddControllers();
            services.ConfigureDbContext(configuration)
                .AddEndpointsApiExplorer()
                .AddHttpClient()
                .ConfigureSwagger()
                .ConfigureVersioning()
                .ConfigureJwtAndSettings(configuration)
                .ConfigureServiceAndRepo()
                .ConfigureHangfire(configuration)
                .AddScoped<INotificationService, NotificationService>()
                .AddScoped<IRepositoryManager, RepositoryManager>()
                .Configure<CloudinaryConfig>(configuration.GetSection("Cloudinary"))
                .AddScoped<ICloudinaryService, CloudinaryService>();

            services.AddCors(options =>
            {
                options.AddPolicy("Frontend", policy =>
                {
                    policy
                        .WithOrigins("https://afrimine-client-latest.onrender.com") // your frontend URL
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }

        private static IServiceCollection ConfigureServiceAndRepo(this IServiceCollection services)
        {
            return services
                .AddScoped<IServiceManager, ServiceManager>()
                .AddScoped<INotificationService, NotificationService>();
        }

        private static IServiceCollection ConfigureJwtAndSettings(this IServiceCollection services,
                                                                IConfiguration configuration)
        {
            var section = configuration.GetSection("AppSettings") ??
               throw new ArgumentNullException("AppSettings");

            var settings = section.Get<AppConfig>() ?? 
                throw new ArgumentNullException("AppSettings");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = settings.JwtIssuer,

                    ValidateAudience = true,
                    ValidAudience = settings.JwtAudience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.JwtKey))
                };
            });

            services.AddAuthorization();

            return services.Configure<AppConfig>(section);
        }

        private static IServiceCollection ConfigureSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen(options =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Afrimine API",
                    Description = "Afrimine API v1.0",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Afrimine",
                        Email = "info@afrimine.com",
                        Url = new Uri("https://afrimine.com")
                    }
                });

                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Title = "Afrimine API",
                    Description = "Afrimine API v2.0",
                    Version = "v2",
                    Contact = new OpenApiContact
                    {
                        Name = "Afrimine",
                        Email = "info@afrimine.com",
                        Url = new Uri("https://afrimine.com")
                    }
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {JWT Token}"
                });

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
                        Array.Empty<string>()
                    }
                });
            });
        }

        private static IServiceCollection ConfigureVersioning(this IServiceCollection services)
        {
            var versioningBuilder = services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new UrlSegmentApiVersionReader());
            });

            versioningBuilder.AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }

        private static IServiceCollection ConfigureDbContext(this IServiceCollection services,
                                                            IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Default") ?? 
                throw new ArgumentNullException("ConnectionString");

            services.AddDbContext<AppDbContext>(option => 
                option.UseNpgsql(connectionString));

            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(60);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager();

            return services; 
        }

        public static IServiceCollection ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(opt =>
                    {
                        opt.UseNpgsqlConnection(configuration.GetConnectionString("Default"));
                    }, new PostgreSqlStorageOptions
                    {
                        SchemaName = "afrimine-hangfire",
                        PrepareSchemaIfNecessary = true
                    })
                    .UseFilter(new AutomaticRetryAttribute()
                    {
                        Attempts = 5,
                        DelayInSecondsByAttemptFunc = _ => 60
                    });
            }).AddHangfireServer(opt =>
            {
                opt.ServerName = "afrimine API";
                opt.SchedulePollingInterval = TimeSpan.FromSeconds(30);
                opt.WorkerCount = 5;
            });

            return services;
        }
    }
}
