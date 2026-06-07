using System.Text;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RentalService.Api.OpenApiConfiguration;
using RentalService.Api.Requests;
using RentalService.Application.Abstractions.Security;
using RentalService.Application.Authorization;
using RentalService.Application.Common;
using RentalService.Application.Exceptions;
using RentalService.Application.Features.Rentals.CreateRental;
using RentalService.Application.Features.Rentals.EndRental;
using RentalService.Application.Features.Rentals.GetRental;
using RentalService.Application.Features.Rentals.GetRentals;
using RentalService.Application.Features.Rentals.RenewRental;
using RentalService.Application.Features.Rentals.StartRental;
using RentalService.Domain.DomainEvents;
using RentalService.Domain.Payments;
using RentalService.Domain.Rentals;
using RentalService.Domain.Services;
using RentalService.Infrastructure;
using RentalService.Infrastructure.Common;
using RentalService.Infrastructure.DomainEvents;
using RentalService.Infrastructure.Messaging;
using RentalService.Infrastructure.Messaging.Consumers;
using RentalService.Infrastructure.Repositories;
using RentalService.Infrastructure.Security;
using RentalService.Infrastructure.Services.ExternalServices;
using RentalService.Infrastructure.Services.InternalServices;
using RentalService.Infrastructure.Services.PricingPolicyServices;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<RentalServiceContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(CreateRentalRequest).Assembly,
    typeof(EndRentalCommandHandler).Assembly,
    typeof(Program).Assembly));

builder.Services.AddAutoMapper(cfg => { }, typeof(RentalCarResponse).Assembly);

builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IIntegrationEventPublisher, MassTransitIntegrationEventPublisher>();
builder.Services.AddScoped<IPricingPoliciesFactory, PricingPoliciesFactory>();
builder.Services.AddScoped<IJsonPriceSettingProvider, JsonPriceSettingProvider>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<RentalPricingDomainService>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<IDomainEventHandler<RentStartedDomainEvent>, RentStartedDomainEventHandler>();
builder.Services.AddScoped<IJwtProvider, InternalJwtProvider>();

builder.Services.AddHttpClient("UserApi", client =>
{
    client.BaseAddress = new Uri("http://user-service:8080");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddHttpClient("CarApi", client =>
{
    client.BaseAddress = new Uri("http://car-service:8080");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<IUserExternalService, UserExternalService>();
builder.Services.AddScoped<ICarExternalService, CarExternalService>();

builder.Services.AddScoped<IClientContext, ClientContext>();
builder.Services.AddScoped<IRentalAuthorizationPolicy, RentalAuthorizationPolicy>();
builder.Services.AddScoped<IRentalAuthorizationService, RentalAuthorizationService>();

builder.Services.AddAuthorization(cfg =>
{
    foreach (var permission in Permissions.AllPermissions)
    {
        cfg.AddPolicy(permission, policy => policy.RequireClaim("permissions", permission));
    }
    cfg.AddPolicy("ContractServiceOnly", policy => policy.RequireClaim("service", "ContractService"));
    cfg.AddPolicy("PaymentServiceOnly", policy => policy.RequireClaim("service", "PaymentService"));
});

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
    busConfigurator.AddConsumer<ContractSignedConsumer>();
    busConfigurator.AddConsumer<DepositPaidConsumer>();

    busConfigurator.UsingRabbitMq((context, configurator)=>
    {
        string host = builder.Configuration["MessageBroker:Host"] ?? "localhost";
        ushort port = builder.Configuration.GetValue<ushort>("MessageBroker:Port", 5672); 
        
        configurator.Host(host, port, "/", h =>
        {
            h.Username(builder.Configuration["MessageBroker:User"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });

        configurator.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (ForbiddenException)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Forbidden" });
    }
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();