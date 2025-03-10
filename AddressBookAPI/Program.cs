using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;

var builder = WebApplication.CreateBuilder(args);

//Register Database 
var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
builder.Services.AddDbContext<AddressAppContext>(options =>
    options.UseSqlServer(connectionString));



// Add services to the container.

builder.Services.AddControllers();


var app = builder.Build();







// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
