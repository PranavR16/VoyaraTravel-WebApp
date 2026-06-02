using AutoMapper;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Voyara.API.Extensions;
using Voyara.API.Middleware;
using Voyara.Core;
using Voyara.Infrastructure;
using Voyara.Infrastructure.Data;
using Voyara.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Extensions
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddRedisCache(builder.Configuration);
builder.Services.AddFluentValidationServices();
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddDbContext<VoyaraDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Voyara.Infrastructure") // Critical for cross-project migrations
    ));

// CORS — allow Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("VoyaraPolicy", policy =>
        policy
            .WithOrigins("http://localhost:4200",
                         "https://voyara.com")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────
app.UseMiddleware<ExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("VoyaraPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── Seed data ─────────────────────────────────────
using var scope = app.Services.CreateScope();
await DataSeeder.SeedAsync(scope.ServiceProvider);

app.Run();