using AutoMapper;
using StackExchange.Redis;
using BusinessLayer.Mapping;
using BusinessLayer.Validators;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using FluentValidation.AspNetCore;
using FluentValidation;
using ModelLayer;
using RepositoryLayer.Entity;
using BusinessLayer.Interface;
using BusinessLayer.Consumers;
using BusinessLayer.Service;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BusinessLayer.Helpers;

using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

Task.Run(() => UserRegisteredConsumer.Consume());
Task.Run(() => ContactAddedConsumer.Consume());




var smtpUser = Environment.GetEnvironmentVariable("SMTP_USERNAME");
var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTP"));
builder.Services.AddSingleton(new SmtpSettings
{
    Host = "smtp.gmail.com",
    Port = 587,
    EnableSSL = true,
    UserName = smtpUser,
    Password = smtpPass
});


var redisConnection = builder.Configuration.GetConnectionString("Redis");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    return ConnectionMultiplexer.Connect("localhost:6379");
});

builder.Services.AddSingleton<RedisCacheService>();


// ✅ JWT Service Register Karo
builder.Services.AddSingleton<JwtTokenGenerator>();

// ✅ JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ClockSkew = TimeSpan.Zero
    };
});


//Register FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AddressBookValidator>();


// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));


// Register Database Connection
var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
builder.Services.AddDbContext<AddressAppContext>(options =>
    options.UseSqlServer(connectionString));


// Register Business & Repository Layer Services
builder.Services.AddScoped<IAddressBookRepository, AddressBookRepository>();
builder.Services.AddScoped<IAddressBookService, AddressBookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<RabbitMQService>();





var app = builder.Build();


// Configure Middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
