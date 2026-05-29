using System.Text;
using CarService.Application.Features.GetCars;
using CarService.Domain.Cars;
using CarService.Infrastructure;
using CarService.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddDbContext<CarServiceDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(GetCarsQuery).Assembly,
    typeof(Program).Assembly));
builder.Services.AddAutoMapper(cfg => { }, typeof(CarListResponse).Assembly, typeof(Program).Assembly);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RentalServiceOnly", policy =>
    {
        policy.RequireClaim("service", "RentalService");
    });
    options.AddPolicy("ContractServiceOnly", policy =>
    {
        policy.RequireClaim("service", "ContractService");
    });
});
builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddAuthentication(options => { })
    .AddJwtBearer("InternalAuth", options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["InternalJwt:Issuer"],
            ValidAudience = builder.Configuration["InternalJwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["InternalJwt:SecretKey"]))
        };
        
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Здесь вы увидите точную причину (например, "Token expired" или "Invalid signature")
                Console.WriteLine("Ошибка JWT: " + context.Exception.Message);
                return Task.CompletedTask;
            }
        }; 
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();