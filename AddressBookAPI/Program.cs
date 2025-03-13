using AutoMapper;
using BusinessLayer.Mapping;
using BusinessLayer.Validators;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using FluentValidation.AspNetCore;
using FluentValidation;
using RepositoryLayer.Entity;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();


// Configure Middleware
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
