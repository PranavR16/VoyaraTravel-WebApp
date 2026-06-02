using Voyara.Core;
using Voyara.Infrastructure;
using Voyara.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Voyara.API.Extensions
{
    public static class ServiceExtensions
    {
        // ── SQL Server Database ───────────────────────────────
        public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration config)
        {
            services.AddDbContext<VoyaraDbContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("Voyara.Infrastructure");
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 3,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    }
                )
            );

            return services;
        }

        // ── JWT Authentication ────────────────────────────────
        public static IServiceCollection AddJwtAuth(
            this IServiceCollection services,
            IConfiguration config)
        {
            var secret = config["Jwt:Secret"]
                ?? throw new InvalidOperationException("JWT Secret not configured");

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                                                       Encoding.UTF8.GetBytes(secret)),
                        ValidateIssuer = true,
                        ValidIssuer = config["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = config["Jwt:Audience"],
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            if (ctx.Exception is SecurityTokenExpiredException)
                                ctx.Response.Headers.Append(
                                    "Token-Expired", "true");
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();
            return services;
        }

        // ── Application Services (DI) ─────────────────────────
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // Repositories
            services.AddScoped(typeof(IGenericRepository<>),
                               typeof(GenericRepository<>));
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IBookingRepository, BookingRepository>();

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

        // ── Redis Cache ───────────────────────────────────────
        public static IServiceCollection AddRedisCache(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["Redis:ConnectionString"];
                options.InstanceName = "Voyara:";
            });

            return services;
        }

        // ── FluentValidation ──────────────────────────────────
        public static IServiceCollection AddFluentValidationServices(
            this IServiceCollection services)
        {
            services
                .AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining<RegisterValidator>()
                .AddValidatorsFromAssemblyContaining<CreatePackageValidator>()
                .AddValidatorsFromAssemblyContaining<CreateBookingValidator>();

            return services;
        }

        // ── Swagger / OpenAPI ─────────────────────────────────
        public static IServiceCollection AddSwaggerDocs(
            this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "🌿 Voyara Travel API",
                    Version = "v1",
                    Description = "Backend API for Voyara luxury travel booking platform",
                    Contact = new OpenApiContact
                    {
                        Name = "Voyara Team",
                        Email = "dev@voyara.com"
                    }
                });

                // JWT bearer auth in Swagger UI
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
                });
            });

            return services;
        }

        // ── CORS ──────────────────────────────────────────────
        public static IServiceCollection AddCorsPolicy(
            this IServiceCollection services,
            IConfiguration config)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("VoyaraPolicy", policy =>
                    policy
                        .WithOrigins(
                            config["Frontend:Url"] ?? "http://localhost:4200",
                            "https://voyara.com",
                            "https://www.voyara.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
            });

            return services;
        }

        //public static class ServiceExtensions
        //{
        //    // ── Database ──────────────────────────────────
        //    public static IServiceCollection AddDatabase(
        //        this IServiceCollection services,
        //        IConfiguration config)
        //    {
        //        services.AddDbContext<VoyaraDbContext>(options =>
        //            options.UseSqlServer(
        //                config.GetConnectionString("DefaultConnection"),
        //                b => b.MigrationsAssembly("Voyara.Infrastructure")));
        //        return services;
        //    }

        //    // ── JWT Auth ──────────────────────────────────
        //    public static IServiceCollection AddJwtAuth(
        //        this IServiceCollection services,
        //        IConfiguration config)
        //    {
        //        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //            .AddJwtBearer(options =>
        //            {
        //                options.TokenValidationParameters = new TokenValidationParameters
        //                {
        //                    ValidateIssuerSigningKey = true,
        //                    IssuerSigningKey = new SymmetricSecurityKey(
        //                        Encoding.UTF8.GetBytes(config["Jwt:Secret"]!)),
        //                    ValidateIssuer = true,
        //                    ValidIssuer = config["Jwt:Issuer"],
        //                    ValidateAudience = true,
        //                    ValidAudience = config["Jwt:Audience"],
        //                    ValidateLifetime = true,
        //                    ClockSkew = TimeSpan.Zero
        //                };
        //            });
        //        services.AddAuthorization();
        //        return services;
        //    }

        //    // ── App Services ──────────────────────────────
        //    public static IServiceCollection AddApplicationServices(
        //        this IServiceCollection services)
        //    {
        //        services.AddScoped<IAuthService, AuthService>();
        //        services.AddScoped<IPackageService, PackageService>();
        //        services.AddScoped<IBookingService, BookingService>();
        //        services.AddScoped<IPaymentService, PaymentService>();
        //        services.AddScoped<IEmailService, EmailService>();
        //        services.AddScoped<ITokenService, TokenService>();
        //        //services.AddScoped<ICacheService, CacheService>();
        //        return services;
        //    }

        //    // ── Redis ─────────────────────────────────────
        //    public static IServiceCollection AddRedisCache(
        //        this IServiceCollection services,
        //        IConfiguration config)
        //    {
        //        services.AddStackExchangeRedisCache(options =>
        //            options.Configuration = config["Redis:ConnectionString"]);
        //        return services;
        //    }

        //    // ── Swagger ───────────────────────────────────
        //    public static IServiceCollection AddSwagger(
        //        this IServiceCollection services)
        //    {
        //        services.AddSwaggerGen(c =>
        //        {
        //            c.SwaggerDoc("v1", new() { Title = "Voyara API", Version = "v1" });
        //            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        //            {
        //                Type = SecuritySchemeType.Http,
        //                Scheme = "bearer"
        //            });
        //        });
        //        return services;
        //    }
        //}
    }
}