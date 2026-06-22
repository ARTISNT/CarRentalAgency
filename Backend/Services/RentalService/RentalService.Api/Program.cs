using System.Text;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RentalService.Api.BackgroundServices;
using RentalService.Api.OpenApiConfiguration;
using RentalService.Api.Requests;
using RentalService.Application.Abstractions;
using RentalService.Application.Abstractions.Security;
using RentalService.Application.Authorization;
using RentalService.Application.Common;
using RentalService.Application.Exceptions;
using RentalService.Application.Features.Rentals.CancelRental;
using RentalService.Application.Features.Rentals.CreateRental;
using RentalService.Application.Features.Rentals.EndRental;
using RentalService.Application.Features.Rentals.GetRental;
using RentalService.Application.Features.Rentals.GetRentals;
using RentalService.Application.Features.Rentals.RenewRental;
using RentalService.Application.Features.Rentals.ScheduleRental;
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
using RentalService.Infrastructure.Clients;
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

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(CreateRentalRequest).Assembly,
        typeof(EndRentalCommandHandler).Assembly,
        typeof(Program).Assembly);

    cfg.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
});

builder.Services.AddAutoMapper(cfg => { }, typeof(RentalCarResponse).Assembly);

builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IIntegrationEventPublisher, MassTransitIntegrationEventPublisher>();
builder.Services.AddScoped<IPricingPoliciesFactory, PricingPoliciesFactory>();
builder.Services.AddScoped<IJsonPriceSettingProvider, JsonPriceSettingProvider>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<RentalPricingDomainService>();
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<IDomainEventHandler<RentStartedDomainEvent>, RentStartedDomainEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<RentScheduledDomainEvent>, RentScheduledDomainEventHandler>();
builder.Services.AddScoped<IDomainEventHandler<RentCancelledDomainEvent>, RentCancelledDomainEventHandler>();
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

builder.Services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(client =>
{
    client.BaseAddress = new Uri("http://payment-service:8080");
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
    busConfigurator.AddConsumer<ContractCreationFaultConsumer>();
    busConfigurator.AddConsumer<FinePaidConsumer>();

    busConfigurator.UsingRabbitMq((context, configurator)=>
    {
        string host = builder.Configuration["MessageBroker:Host"] ?? "localhost";
        ushort port = builder.Configuration.GetValue<ushort>("MessageBroker:Port", 5672);


        configurator.Host(host, port, "/", h =>
        {
            h.Username(builder.Configuration["MessageBroker:User"]);
            h.Password(builder.Configuration["MessageBroker:Password"]);
        });

        configurator.UseMessageRetry(r => r
            .Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(3))
            .Handle<Exception>(ex => ex is not InvalidOperationException or KeyNotFoundException));

        configurator.ReceiveEndpoint("rental-service-contract-signed", e =>
        {
            e.ConfigureConsumer<ContractSignedConsumer>(context);
        });
        configurator.ReceiveEndpoint("rental-service-deposit-paid", e =>
        {
            e.ConfigureConsumer<DepositPaidConsumer>(context);
        });
        configurator.ReceiveEndpoint("rental-service-contract-ended", e =>
        {
            e.ConfigureConsumer<ContractEndedConsumer>(context);
        });
        configurator.ReceiveEndpoint("rental-service-contract-creation-fault", e =>
        {
            e.ConfigureConsumer<ContractCreationFaultConsumer>(context);
        });
        configurator.ReceiveEndpoint("rental-service-fine-paid", e =>
        {
            e.ConfigureConsumer<FinePaidConsumer>(context);
        });

        configurator.ConfigureEndpoints(context);
    });
});

builder.Services.AddHostedService<RentalActivationService>();
builder.Services.AddHostedService<RentalExpirationService>();

var app = builder.Build();

// Apply pending EF Core migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<RentalServiceContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<RentalServiceContext>>();
    await RentalService.Api.Common.MigrationRunner.RunWithRetryAsync(db, logger);
}

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
    catch (PassportRequiredException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "PassportRequired",
            message = ex.Message
        });
    }
    catch (UnpaidFineException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            error = "UnpaidFine",
            message = ex.Message,
            outstandingAmount = ex.OutstandingAmount
        });
    }
    catch (ForbiddenException)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "Forbidden" });
    }
    catch (Contracts.Common.AccountDeactivatedException)
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { error = "account_deactivated" });
    }
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();