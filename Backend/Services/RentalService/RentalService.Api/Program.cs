using MassTransit;
using Microsoft.EntityFrameworkCore;
using RentalService.Api.Requests;
using RentalService.Application.Common;
using RentalService.Application.Features.Rentals.CreateRental;
using RentalService.Application.Features.Rentals.EndRental;
using RentalService.Application.Features.Rentals.GetRental;
using RentalService.Application.Features.Rentals.GetRentals;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Domain.Services;
using RentalService.Infrastructure;
using RentalService.Infrastructure.Common;
using RentalService.Infrastructure.Repositories;
using RentalService.Infrastructure.Services.ExternalServices;
using RentalService.Infrastructure.Services.PricingPolicyServices;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

builder.Services.AddDbContext<RentalServiceContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(CreateRentalRequest).Assembly,
    typeof(EndRentalCommandHandler).Assembly,
    typeof(Program).Assembly));

builder.Services.AddAutoMapper(cfg => { }, typeof(RentalCarResponse).Assembly);

builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IPricingPoliciesFactory, PricingPoliciesFactory>();
builder.Services.AddScoped<IJsonPriceSettingProvider, JsonPriceSettingProvider>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<RentalPricingDomainService>();
builder.Services.AddHttpClient("UserApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5068");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient("CarApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5063");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<IUserExternalService, UserExternalService>();
builder.Services.AddScoped<ICarExternalService, CarExternalService>();
/*
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    
    busConfigurator.UsingRabbitMq((context, configurator)=>
    {
        configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]), h =>
        {
            h.Username(builder.Configuration["MessageBroker:Username"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });
        
    });
});*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();