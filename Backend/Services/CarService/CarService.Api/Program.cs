using System.Text;
using CarService.Application.Abstractions.Security;
using CarService.Application.Authorization;
using CarService.Application.Common;
using CarService.Application.Features.GetCars;
using CarService.Domain.Cars;
using CarService.Infrastructure;
using CarService.Infrastructure.Messaging.Consumers;
using CarService.Infrastructure.Persistence.Repositories;
using CarService.Infrastructure.Security;
using CarService.OpenApiConfiguration;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddControllers();
builder.Services.AddDbContext<CarServiceDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(GetCarsQuery).Assembly,
    typeof(Program).Assembly));
builder.Services.AddAutoMapper(cfg => { }, typeof(CarListResponse).Assembly, typeof(Program).Assembly);

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in Permissions.AllPermissions)
    {
        options.AddPolicy(permission, policy => policy.RequireClaim("permissions", permission));
    }

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
builder.Services.AddScoped<IClientContext, ClientContext>();
builder.Services.AddScoped<ICarAuthorizationPolicy, CarAuthorizationPolicy>();
builder.Services.AddScoped<ICarAuthorizationService, CarAuthorizationService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = "UserAuth";
        options.DefaultChallengeScheme = "UserAuth";
    })
    .AddJwtBearer("UserAuth", options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["UserJwt:Issuer"],
            ValidAudience = builder.Configuration["UserJwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["UserJwt:SecretKey"]))
        };
    })
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
    });

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    busConfigurator.AddConsumer<ContractEndedConsumer>();
    busConfigurator.AddConsumer<RentalStartedConsumer>();
    busConfigurator.AddConsumer<RentalScheduledConsumer>();
    busConfigurator.AddConsumer<RentalCancelledConsumer>();
    busConfigurator.AddConsumer<RentalEndedConsumer>();

    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        string host = builder.Configuration["MessageBroker:Host"] ?? "localhost";
        ushort port = builder.Configuration.GetValue<ushort>("MessageBroker:Port", 5672);

        configurator.Host(host, port, "/", h =>
        {
            h.Username(builder.Configuration["MessageBroker:User"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });

        configurator.ReceiveEndpoint("car-service-contract-ended", e =>
        {
            e.ConfigureConsumer<ContractEndedConsumer>(context);
        });
        configurator.ReceiveEndpoint("car-service-rental-started", e =>
        {
            e.ConfigureConsumer<RentalStartedConsumer>(context);
        });
        configurator.ReceiveEndpoint("car-service-rental-scheduled", e =>
        {
            e.ConfigureConsumer<RentalScheduledConsumer>(context);
        });
        configurator.ReceiveEndpoint("car-service-rental-cancelled", e =>
        {
            e.ConfigureConsumer<RentalCancelledConsumer>(context);
        });
        configurator.ReceiveEndpoint("car-service-rental-ended", e =>
        {
            e.ConfigureConsumer<RentalEndedConsumer>(context);
        });

        configurator.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Apply pending EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CarServiceDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<CarServiceDbContext>>();
    await CarService.Api.Common.MigrationRunner.RunWithRetryAsync(db, logger);
}

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