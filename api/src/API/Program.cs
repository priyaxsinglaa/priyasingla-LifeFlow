using API.Models;
using API.Services;
using API.Services.Interfaces;
using API.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LifeFlowDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourDefaultSecretKeyGoesHere";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization(); 
builder.Services.AddHttpClient("", client =>
{
    client.Timeout = TimeSpan.FromMinutes(5); // AI ko generate karne ka poora time dein
});
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDonationService, DonationService>();
builder.Services.AddScoped<IBloodStockService, BloodStockService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<IForecastingService, ForecastingService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddSingleton<JwtHelper>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Your Angular URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
